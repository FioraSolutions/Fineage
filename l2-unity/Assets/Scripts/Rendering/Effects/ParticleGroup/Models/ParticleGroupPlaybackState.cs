internal sealed partial class ParticleGroupPlaybackState
{
    public bool Stopped = true;
    public bool SpawnStopped;
    public float LastEnable;
    public float LastLoop;
    public int ParticleIndex;
    public int SpawnedCount;
    public bool BurstFinished;
    public float BaseShaderLifetime = -1f;
    public bool RuntimeContinuousLoop;
    public bool HasLoopOverride;
    public bool LoopOverrideValue;
    public uint MeshRandBase;
    public uint SpriteRandBase;
    public bool RuntimeClonesCreated;
    public string FirstPlayStack;






}
