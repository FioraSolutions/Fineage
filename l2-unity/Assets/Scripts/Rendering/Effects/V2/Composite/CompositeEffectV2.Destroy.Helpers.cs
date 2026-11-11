using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public partial class CompositeEffectV2 : CompositePrefabEffect
{
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
}
