
using UnityEngine;

public partial class ParticleSingleLifetimeTracker
{
    public void ResetForPlayPart(float now)
    {
        LastEnable = now;
        LastLoop = 0f;
        SpawnedAt = 0f;
        SpawnedCount = 0;
        Active = false;
        Stopped = false;
    }

    public void MarkSpawned(float now)
    {
        Active = true;
        SpawnedAt = now;
    }

    public bool IsBeforeStartDelay(float now, float startDelay)
    {
        return now - LastEnable < startDelay;
    }

    public bool IsSlotExpired(float now, bool preserveShaderTimeInContinuousLoop)
    {
        return Active && !preserveShaderTimeInContinuousLoop && now - SpawnedAt >= Duration;
    }
}
