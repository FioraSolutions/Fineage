using UnityEngine;

public readonly struct EffectPlaybackContext
{
    public readonly float Now;
    public readonly float CastStartTime;
    public readonly float HitTime;
    public readonly float FlightTime;
    public readonly float ServerTimeToShoot;
    public readonly int CastId;
    public readonly uint Seed;

    public EffectPlaybackContext(
        float now,
        float castStartTime,
        float hitTime,
        float flightTime,
        float serverTimeToShoot,
        int castId,
        uint seed)
    {
        Now = now;
        CastStartTime = castStartTime;
        HitTime = hitTime;
        FlightTime = flightTime;
        ServerTimeToShoot = serverTimeToShoot;
        CastId = castId;
        Seed = seed;
    }

    public static EffectPlaybackContext FromCast(MagicCastData castData, float now, uint seed)
    {
        if (castData == null)
        {
            return new EffectPlaybackContext(now, now, 0f, 0f, 0f, 0, seed);
        }

        return new EffectPlaybackContext(
            now,
            castData.StartTime > 0f ? castData.StartTime : now,
            castData.HitTime,
            castData.FlightTime,
            castData.serverTimeToShoot,
            0,
            seed);
    }
}

public struct ParticleGroupAuthoring
{
    public Renderer[] particles;
    public float startDelay;
    public int countPerSecond;
    public int maxCount;
    public bool cloneToMaxCount;
    public int cloneLimit;
    public bool useGpuInstancing;
    public bool isBurstSpawning;
    public float relativeWarmupTime;
    public float warmupTicksPerSecond;
    public float duration;
    public bool hasFixedDuration;
    public bool instantKillAtCastEnd;
    public bool respawnDeadParticles;
    /// <summary>NPC deco: emit until Stop, not until a cast window.</summary>
    public bool hostOwnedEmission;
    /// <summary>L2 AdjustparticleLife: one slot lives until the emission window.</summary>
    public bool stretchParticleLifeToWindow;
    public float authoredParticleLife;
    public float targetParticleLife;
    public L2ParticleCoordinateSystem coordinateSystem;
}
