using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;

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
