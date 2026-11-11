public sealed partial class ParticleGroupPlaybackState
{
    public bool ShouldLoopContinuously(bool forceContinuous) =>
        forceContinuous || RuntimeContinuousLoop;

    public void Begin(float now, int countPerSecond, bool hasFixedDuration, float duration)
    {
        LastEnable = now;
        LastLoop = now - (countPerSecond > 0 ? 1f / countPerSecond : 0.05f);
        ParticleIndex = 0;
        SpawnedCount = 0;
        BurstFinished = false;
        Stopped = false;
        SpawnStopped = false;
        MeshRandBase = L2MaterialPropertyCopier.CreateFiniteAppRandState();
        SpriteRandBase = L2MaterialPropertyCopier.CreateFiniteAppRandState();
        // Non-fixed groups use the server/cast duration. If that duration is
        // longer than one shader cycle, keep feeding their authored slots until
        // StopPart. Target one-shots override this to false in their composite.
        RuntimeContinuousLoop = !hasFixedDuration && duration > BaseShaderLifetime + 0.05f;
        if (HasLoopOverride)
            RuntimeContinuousLoop = LoopOverrideValue;
    }

    public void SetLoopOverride(bool hasOverride, bool value)
    {
        HasLoopOverride = hasOverride;
        LoopOverrideValue = value;
    }
}
