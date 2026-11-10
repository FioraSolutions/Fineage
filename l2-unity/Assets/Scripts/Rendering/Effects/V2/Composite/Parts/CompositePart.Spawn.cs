[System.Serializable]
public sealed class HomeFlightPart : CompositePart
{
    public ProjectileLaunchMode launchMode = ProjectileLaunchMode.OnAnimationShoot;
    // Default outbound home-flight speed. m_u003_b overwrites this from the
    // live lock in L2EffectGeneratorAssetOverrides, not from UC Speed.
    public float speed = 5f;
    public float acceleration;
    [HideInInspector] public float maxSpeed;
    [HideInInspector] public float fadeStartDistance = 0.5f;
    [HideInInspector] public float fadeOutSeconds = 0.35f;
    [HideInInspector] public float arriveDistance = 0.2f;
    [HideInInspector] public float maxLifetime;
    public EffectAttachmentPoint homeAttachmentPoint = EffectAttachmentPoint.CasterCenter;
    public Vector3 homeOffset = new Vector3(0f, 0.1f, 0f);
    public bool usePathArc = true;
    [HideInInspector] public float pathStartLineFactor = -0.15f;
    [HideInInspector] public float pathPeelAlongLine = 0.16f;
    [HideInInspector] public float pathApexAlongLine;
    [HideInInspector] public float pathPeakHeightAlongLine;
    [HideInInspector] public float pathSideOffset = 1.25f;
    [HideInInspector] public float pathHeightOffset = 0.44f;
    [HideInInspector] public float pathDistanceHeightFactor = 0.112f;
    [HideInInspector] public float pathEarlyClimbFactor = 0.2f;
    [HideInInspector] public float pathAscentSpeedScale = 1f;
    [HideInInspector] public float pathDescentSpeedScale = 1f;
    public bool rotateToVelocity = true;
    public bool destroyOnArrive = true;
    [HideInInspector] public bool mirrorDualFlight;

    public override bool WantsAnimationShoot =>
        spawnTiming == CompositePartSpawnTiming.OnAnimationShoot ||
        launchMode == ProjectileLaunchMode.OnAnimationShoot;

    public override bool IsLaunchedProjectile =>
        launchMode == ProjectileLaunchMode.Immediate ||
        launchMode == ProjectileLaunchMode.OnAnimationShoot;

    public override bool UsesCastHitLifetime => false;

    public override bool OutlivesComposite => true;

    public override void ConfigurePlayback(
        BaseEffect instance,
        EffectSettings settings,
        MagicCastData castData,
        EffectResolveContext context)
    {
        // UC SpriteEmitter5: Lifetime=0.01 + 3000 pps + MaxParticles=3. Live
        // recycles those slots for the whole FNMover trip. Fixed-duration
        // burst would spawn once and vanish in a frame (SpriteEmitter2's
        // 0.33s trail still reads; the core does not).
        ParticleEmitterV2.BindHostOwnedEmission(instance);
    }

    public bool ShouldLaunchImmediately => launchMode == ProjectileLaunchMode.Immediate;

    public bool ShouldLaunchOnShoot => launchMode == ProjectileLaunchMode.OnAnimationShoot;

    public override string Describe()
    {
        return base.Describe() + " homeFlight launch=" + launchMode +
               " speed=" + speed.ToString("0.###") +
               " accel=" + acceleration.ToString("0.##") +
               " max=" + maxSpeed.ToString("0.##") +
               " side=" + pathSideOffset.ToString("0.##") +
               " height=" + pathHeightOffset.ToString("0.##") +
               " dual=" + (mirrorDualFlight ? "1" : "0");
    }

    public override void OnAfterSpawn(BaseEffect instance, EffectResolveContext context)
    {
        if (instance == null)
        {
            return;
        }

        instance.PrepareDestroyOnHomeArrival();
        EffectPart[] parts = instance.GetComponentsInChildren<EffectPart>(true);
        for (int i = 0; i < parts.Length; i++)
        {
            if (parts[i] != null)
            {
                parts[i].SetOwnerWorldPosOverride(true, instance.transform.position);
            }
        }

        if (ShouldHideUntilShoot())
        {
            CompositeProjectileLaunchHelper.SetPartVisualsVisible(instance.transform, false);
        }
    }

    public override void OnAnimationShoot(BaseEffect instance, EffectResolveContext context)
    {
        if (instance == null)
        {
            return;
        }

        if (ShouldHideUntilShoot())
        {
            L2Particle particle = instance as L2Particle;
            if (particle != null)
            {
                particle.ResetTimer();
            }

            CompositeProjectileLaunchHelper.SetPartVisualsVisible(instance.transform, true);
        }
    }

    public bool TryLaunch(BaseEffect instance, EffectResolveContext context, IHomeProjectileService homeProjectiles)
    {
        if (instance == null || context == null || context.CasterTransform == null || homeProjectiles == null)
        {
            return false;
        }

        homeProjectiles.EnsureDualFlightRoots(instance, mirrorDualFlight);
        return homeProjectiles.Launch(instance, context, BuildConfig());
    }

    CompositeHomeProjectileConfig BuildConfig()
    {
        return new CompositeHomeProjectileConfig
        {
            launchMode = launchMode,
            speed = speed,
            acceleration = acceleration,
            maxSpeed = maxSpeed,
            fadeStartDistance = fadeStartDistance,
            fadeOutSeconds = fadeOutSeconds,
            arriveDistance = arriveDistance,
            maxLifetime = maxLifetime,
            homeAttachmentPoint = homeAttachmentPoint,
            homeOffset = homeOffset,
            usePathArc = usePathArc,
            pathStartLineFactor = pathStartLineFactor,
            pathPeelAlongLine = pathPeelAlongLine,
            pathApexAlongLine = pathApexAlongLine,
            pathPeakHeightAlongLine = pathPeakHeightAlongLine,
            pathSideOffset = pathSideOffset,
            pathHeightOffset = pathHeightOffset,
            pathDistanceHeightFactor = pathDistanceHeightFactor,
            pathEarlyClimbFactor = pathEarlyClimbFactor,
            pathAscentSpeedScale = pathAscentSpeedScale,
            pathDescentSpeedScale = pathDescentSpeedScale,
            rotateToVelocity = rotateToVelocity,
            destroyOnArrive = destroyOnArrive,
            mirrorDualFlight = mirrorDualFlight
        };
    }

    bool ShouldHideUntilShoot()
    {
        if (spawnTiming == CompositePartSpawnTiming.OnHitCollider ||
            spawnTiming == CompositePartSpawnTiming.OnAnimationShoot)
        {
            return false;
        }

        return launchMode == ProjectileLaunchMode.OnAnimationShoot;
    }
}
