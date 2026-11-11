using System.Collections.Generic;
using UnityEngine;

public sealed partial class HomeProjectileLauncher
{
    public HomeProjectileLauncher(
        HomeProjectileDualFlightRoots dualFlightRoots,
        IEffectAttachmentResolver attachmentResolver)
    {
        _dualFlightRoots = dualFlightRoots;
        _attachmentResolver = attachmentResolver;
    }

    public bool Launch(
        BaseEffect effect,
        EffectResolveContext context,
        CompositeHomeProjectileConfig config)
    {
        if (effect == null || context == null || context.CasterTransform == null || config == null)
        {
            return false;
        }

        List<HomeProjectileFlightAnchor> taggedAnchors = _dualFlightRoots.CollectTaggedAnchors(effect);
        if (taggedAnchors.Count > 0)
        {
            return LaunchTaggedAnchors(effect, context, config, taggedAnchors);
        }

        List<ParticleGroup> anchors = CollectFlightAnchors(effect);
        if (anchors.Count == 0)
        {
            return LaunchMover(
                effect.transform,
                effect,
                context,
                config,
                null,
                ParticleGroupHomeFlightProfile.DefaultAnchor);
        }

        HomeProjectileFlightCoordinator coordinator = GetOrAddCoordinator(effect);
        bool anyLaunched = false;
        int launchedCount = 0;
        for (int i = 0; i < anchors.Count; i++)
        {
            ParticleGroup group = anchors[i];
            if (group == null)
            {
                continue;
            }

            group.TryGetHomeFlightProfile(out ParticleGroupHomeFlightProfile profile);
            if (LaunchMover(group.transform, effect, context, config, coordinator, profile))
            {
                anyLaunched = true;
                launchedCount++;
            }
        }

        coordinator.BeginFlight(effect, launchedCount);
        return anyLaunched;
    }

    bool LaunchTaggedAnchors(
        BaseEffect effect,
        EffectResolveContext context,
        CompositeHomeProjectileConfig config,
        List<HomeProjectileFlightAnchor> taggedAnchors)
    {
        HomeProjectileFlightCoordinator coordinator = GetOrAddCoordinator(effect);
        bool anyLaunched = false;
        int launchedCount = 0;
        for (int i = 0; i < taggedAnchors.Count; i++)
        {
            HomeProjectileFlightAnchor anchor = taggedAnchors[i];
            if (anchor == null)
            {
                continue;
            }

            if (LaunchMover(anchor.transform, effect, context, config, coordinator, anchor.profile))
            {
                anyLaunched = true;
                launchedCount++;
            }
        }

        coordinator.BeginFlight(effect, launchedCount);
        return anyLaunched;
    }
}
