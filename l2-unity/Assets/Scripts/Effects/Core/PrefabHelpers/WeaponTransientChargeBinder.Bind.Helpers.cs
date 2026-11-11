using UnityEngine;

public sealed partial class WeaponTransientChargeBinder
{
    void BindAuthoredPlayback()
    {
        BindAllEmittersToAuthoredTiming();
        L2Particle particle = GetComponent<L2Particle>();
        if (particle != null)
        {
            particle.BindLifetimeToAuthoredStreams();
        }
    }

    void BindAllEmittersToAuthoredTiming()
    {
        ParticleGroupV2[] groups = GetComponentsInChildren<ParticleGroupV2>(true);
        for (int i = 0; i < groups.Length; i++)
        {
            if (groups[i] != null)
            {
                groups[i].BindAuthoredTiming();
            }
        }
    }

    Transform ResolveWeapon()
    {
        if (_weapon != null)
        {
            return _weapon;
        }

        return transform.parent;
    }
}
