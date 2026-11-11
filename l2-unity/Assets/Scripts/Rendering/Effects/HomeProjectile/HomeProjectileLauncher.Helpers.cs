using System.Collections.Generic;
using UnityEngine;

public sealed partial class HomeProjectileLauncher
{
    static List<ParticleGroup> CollectFlightAnchors(BaseEffect effect)
    {
        List<ParticleGroup> anchors = new List<ParticleGroup>();
        ParticleGroup[] groups = effect.GetComponentsInChildren<ParticleGroup>(true);
        for (int i = 0; i < groups.Length; i++)
        {
            ParticleGroup group = groups[i];
            if (group != null && group.IsHomeFlightAnchor)
            {
                anchors.Add(group);
            }
        }

        return anchors;
    }

    bool LaunchMover(
        Transform flightTransform,
        BaseEffect effect,
        EffectResolveContext context,
        CompositeHomeProjectileConfig config,
        HomeProjectileFlightCoordinator coordinator,
        ParticleGroupHomeFlightProfile groupProfile)
    {
        if (flightTransform == null)
        {
            return false;
        }

        HomeProjectileMover mover = flightTransform.GetComponent<HomeProjectileMover>();
        if (mover == null)
        {
            mover = flightTransform.gameObject.AddComponent<HomeProjectileMover>();
        }

        mover.Launch(effect, context, config, coordinator, groupProfile, _attachmentResolver);
        return true;
    }

    static HomeProjectileFlightCoordinator GetOrAddCoordinator(BaseEffect effect)
    {
        HomeProjectileFlightCoordinator coordinator = effect.gameObject.GetComponent<HomeProjectileFlightCoordinator>();
        return coordinator != null
            ? coordinator
            : effect.gameObject.AddComponent<HomeProjectileFlightCoordinator>();
    }
}
