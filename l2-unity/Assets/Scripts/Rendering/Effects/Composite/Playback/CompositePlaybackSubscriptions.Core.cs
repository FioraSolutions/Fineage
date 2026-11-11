using System;
using System.Collections.Generic;
using UnityEngine;

public static partial class CompositePlaybackSubscriptions
{
    public static void SubscribeShootIfNeeded(
        CompositePrefabPart[] parts,
        EffectResolveContext context,
        List<AnimationEventsBase> shootEventSources,
        Action<string> handler,
        IAnimationManager animations,
        out AnimationEventsBase animationEvents,
        ref bool isSubscribedToAnyShoot)
    {
        UnsubscribeShoot(shootEventSources, handler, ref isSubscribedToAnyShoot);
        animationEvents = null;

        if (!CompositeProjectileLaunchHelper.RequiresAnimationShootEvent(parts))
        {
            return;
        }

        IAnimationManager animationManager = animations != null ? animations : RenderingPacketServices.Animations;
        if (context?.CasterEntity?.Identity == null || animationManager == null)
        {
            return;
        }

        int casterId = context.CasterEntity.Identity.Id;
        animationEvents = animationManager.GetAnimationEvents(casterId);
        TrySubscribeShootSource(shootEventSources, animationEvents, handler);
        isSubscribedToAnyShoot = true;
    }

    public static void UnsubscribeShoot(
        List<AnimationEventsBase> shootEventSources,
        Action<string> handler,
        ref bool isSubscribedToAnyShoot)
    {
        if (shootEventSources != null)
        {
            for (int i = 0; i < shootEventSources.Count; i++)
            {
                AnimationEventsBase source = shootEventSources[i];
                if (source != null)
                {
                    source.OnAnimationStartShoot -= handler;
                    source.OnAnimationAttackShot -= handler;
                }
            }

            shootEventSources.Clear();
        }

        isSubscribedToAnyShoot = false;
    }

    public static void TrySubscribeShootSource(
        List<AnimationEventsBase> shootEventSources,
        AnimationEventsBase source,
        Action<string> handler)
    {
        if (source == null || shootEventSources == null || shootEventSources.Contains(source))
        {
            return;
        }

        source.OnAnimationStartShoot += handler;
        source.OnAnimationAttackShot += handler;
        shootEventSources.Add(source);
    }
}
