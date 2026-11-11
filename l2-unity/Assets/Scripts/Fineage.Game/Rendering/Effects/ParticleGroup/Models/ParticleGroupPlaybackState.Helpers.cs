internal sealed partial class ParticleGroupPlaybackState
{
    public void StopSpawning()
    {
        RuntimeContinuousLoop = false;
        HasLoopOverride = true;
        LoopOverrideValue = false;
        SpawnStopped = true;
    }

    public void WrapIndex(int slotCount)
    {
        if (slotCount <= 0)
            return;
        if (ParticleIndex >= slotCount)
            ParticleIndex = 0;
    }

    public void AdvanceIndex(int slotCount)
    {
        ParticleIndex++;
        WrapIndex(slotCount);
    }
}
