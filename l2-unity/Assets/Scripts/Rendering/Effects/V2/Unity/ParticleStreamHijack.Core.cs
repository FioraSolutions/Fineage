using System;
using UnityEngine;

public static partial class ParticleStreamHijack
{
    public static void Convert(BaseEffect instance)
    {
        if (instance == null)
        {
            return;
        }

        ParticleGroup[] groups = instance.GetComponentsInChildren<ParticleGroup>(true);
        for (int i = 0; i < groups.Length; i++)
        {
            ConvertGroup(groups[i]);
        }

        ParticleSingle[] singles = instance.GetComponentsInChildren<ParticleSingle>(true);
        for (int i = 0; i < singles.Length; i++)
        {
            ConvertSingle(singles[i]);
        }
    }

    static bool HasV2Emitter(Component host)
    {
        return host != null && host.GetComponent<ParticleGroupV2>() != null;
    }
}
