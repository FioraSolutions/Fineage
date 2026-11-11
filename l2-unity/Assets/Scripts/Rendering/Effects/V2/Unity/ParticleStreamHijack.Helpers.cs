using UnityEngine;

public static partial class ParticleStreamHijack
{
    static void ConvertGroup(ParticleGroup group)
    {
        if (group == null || HasV2Emitter(group))
        {
            return;
        }

        ParticleGroupAuthoring authoring = group.CaptureAuthoring();
        ParticleStreamDriver driver = BindDriver(group.gameObject, authoring);
        driver.OwnerTarget = group.OwnerTarget;
        driver.FollowTarget = group.FollowTarget;
        driver.SurfaceNormal = group.SurfaceNormal;
        group.enabled = false;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        // Debug.Log($"[CompositeEffectV2] Hijack group='{group.name}' rate={authoring.countPerSecond}/s max={authoring.maxCount}");
#endif
    }

    static void ConvertSingle(ParticleSingle single)
    {
        if (single == null || HasV2Emitter(single))
        {
            return;
        }

        ParticleGroupAuthoring authoring = single.CaptureAuthoring();
        ParticleStreamDriver driver = BindDriver(single.gameObject, authoring);
        driver.OwnerTarget = single.OwnerTarget;
        driver.FollowTarget = single.FollowTarget;
        driver.SurfaceNormal = single.SurfaceNormal;
        single.enabled = false;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        // Debug.Log($"[CompositeEffectV2] Hijack single='{single.name}' max={authoring.maxCount}");
#endif
    }

    static ParticleStreamDriver BindDriver(GameObject host, ParticleGroupAuthoring authoring)
    {
        ParticleStreamDriver driver = host.GetComponent<ParticleStreamDriver>();
        if (driver == null)
        {
            driver = host.AddComponent<ParticleStreamDriver>();
        }

        driver.Bind(authoring);
        return driver;
    }
}
