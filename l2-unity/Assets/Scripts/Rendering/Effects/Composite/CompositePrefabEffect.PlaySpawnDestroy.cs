using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class CompositePrefabEffect
{
    protected override void OnTimedCompositeDestroy()
    {
        CompositePlaybackSubscriptions.UnsubscribeShoot(
            _shootEventSources, HandleAnimationShoot, ref _isSubscribedToAnyShoot);
        _animationEvents = null;
        CompositePlaybackSubscriptions.UnsubscribeHit(
            HandleProjectileEffectHit, ref _isSubscribedToProjectileEffectHit);
        StopAndClearCoroutine(ref _pendingSpawnRoutine);
        StopAndClearCoroutine(ref _fallbackShootRoutine);
        StopAndClearCoroutine(ref _visibilityProbeRoutine);

        _pendingParts.Clear();
        _pendingHitColliderParts.Clear();
        _pendingAnimationShootParts.Clear();
        DestroyOwnedSpawnedParts();
        _spawnedPartInstances.Clear();
        _launchedProjectileParts.Clear();
        _launchedHomeProjectileParts.Clear();
    }

    void DestroyOwnedSpawnedParts()
    {
        foreach (KeyValuePair<CompositePrefabPart, BaseEffect> pair in _spawnedPartInstances)
        {
            if (pair.Value == null)
            {
                continue;
            }

            if (_launchedProjectileParts.Contains(pair.Key) ||
                _launchedHomeProjectileParts.Contains(pair.Key))
            {
                continue;
            }

            Destroy(pair.Value.gameObject);
        }
    }
}
