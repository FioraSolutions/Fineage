using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public sealed partial class ParticleGroupSimulation
{
    public void Dispose()
    {
        CompleteExpire();
        DisposeLifetime();
        DisposeGpu();
    }
}
