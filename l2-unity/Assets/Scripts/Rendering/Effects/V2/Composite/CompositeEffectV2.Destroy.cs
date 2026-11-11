using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class CompositeEffectV2 : CompositePrefabEffect
{
    protected override void OnTimedCompositeDestroy()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        LogV2TaDestroyState("before flush");
#endif
        SpawnPendingAnimationShootParts();
        FlushPendingDelayedIndependentParts();
        FlushPendingHitAtCastEnd();
        UnsubscribeV2Shoot();
        UnsubscribeV2Hit();
        StopAndClearCoroutine(ref _v2PendingRoutine);
        StopAndClearCoroutine(ref _v2ShootWatchdogRoutine);
        DestroyOwnedV2Parts();
        ClearV2Runtime();
        base.OnTimedCompositeDestroy();
    }

    void DestroyOwnedV2Parts()
    {
        foreach (KeyValuePair<CompositePart, BaseEffect> pair in _spawned)
        {
            if (pair.Value == null ||
                _launchedProjectiles.Contains(pair.Key) ||
                (pair.Key != null && pair.Key.OutlivesComposite))
            {
                continue;
            }

            Destroy(pair.Value.gameObject);
        }
    }

    void ClearV2Runtime()
    {
        _pendingDelayed.Clear();
        _pendingHitCollider.Clear();
        _pendingAnimationShoot.Clear();
        _spawned.Clear();
        _launchedProjectiles.Clear();
        _v2DirectShootObserved = false;
    }

    static bool HasParticleGroupV2(BaseEffect instance)
    {
        return instance != null &&
               instance.GetComponentInChildren<ParticleGroupV2>(true) != null;
    }

    static bool HasLegacyParticleAuthoring(BaseEffect instance)
    {
        if (instance == null)
        {
            return false;
        }

        ParticleGroup[] groups = instance.GetComponentsInChildren<ParticleGroup>(true);
        for (int i = 0; i < groups.Length; i++)
        {
            if (groups[i] != null &&
                groups[i].GetComponent<ParticleGroupV2>() == null)
            {
                return true;
            }
        }

        ParticleSingle[] singles = instance.GetComponentsInChildren<ParticleSingle>(true);
        for (int i = 0; i < singles.Length; i++)
        {
            if (singles[i] != null &&
                singles[i].GetComponent<ParticleGroupV2>() == null)
            {
                return true;
            }
        }

        return false;
    }

    sealed class PendingV2Part
    {
        public CompositePart Part;
        public float SpawnAtTime;
    }
}
