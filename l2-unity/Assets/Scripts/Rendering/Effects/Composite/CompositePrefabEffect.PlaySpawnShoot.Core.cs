using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class CompositePrefabEffect
{
    void SubscribeShootEventIfNeeded()
    {
        CompositePlaybackSubscriptions.SubscribeShootIfNeeded(
            _parts,
            _context,
            _shootEventSources,
            HandleAnimationShoot,
            Animations,
            out _animationEvents,
            ref _isSubscribedToAnyShoot);
    }

    void SubscribeProjectileHitEventIfNeeded()
    {
        CompositePlaybackSubscriptions.SubscribeHitIfNeeded(
            _parts,
            HandleProjectileEffectHit,
            ref _isSubscribedToProjectileEffectHit);
    }

    void HandleAnimationShoot(string _)
    {
        ProcessShootEvent("direct");
    }
}
