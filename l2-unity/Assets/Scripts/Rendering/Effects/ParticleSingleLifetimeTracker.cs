
using UnityEngine;

public partial class ParticleSingleLifetimeTracker
{
    public float LastEnable { get; set; }
    public float LastLoop { get; set; }
    public float SpawnedAt { get; set; }
    public int SpawnedCount { get; set; }
    public bool Active { get; set; }
    public bool Stopped { get; set; }
    public float Duration { get; set; }
    public float BaseShaderLifetime { get; set; }
    public bool RuntimeContinuousLoop { get; set; }








}
