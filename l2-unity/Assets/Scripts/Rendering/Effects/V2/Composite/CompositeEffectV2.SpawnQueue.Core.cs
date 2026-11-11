using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class CompositeEffectV2
{
    void PrepareV2Playback(BaseEffect instance)
    {
        if (HasParticleGroupV2(instance) && !HasLegacyParticleAuthoring(instance))
        {
            return;
        }

        ParticleStreamHijack.Convert(instance);
    }
}
