using UnityEngine;

[System.Serializable]
public sealed partial class HomeFlightPart : CompositePart
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


    public bool ShouldLaunchImmediately => launchMode == ProjectileLaunchMode.Immediate;

    public bool ShouldLaunchOnShoot => launchMode == ProjectileLaunchMode.OnAnimationShoot;






}
