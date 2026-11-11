using UnityEngine;

// Required for SerializeReference on CompositeEffectV2._v2Parts (lost in hub split #1209).
[System.Serializable]
public sealed partial class ShotProjectilePart : CompositePart
{
    public ProjectileLaunchMode launchMode = ProjectileLaunchMode.OnAnimationShoot;
    public bool showBeforeAnimationShoot;
    public ProjectileImpactType impactType = ProjectileImpactType.EffectOnly;
    public float speed;

    public override bool WantsAnimationShoot =>
        spawnTiming == CompositePartSpawnTiming.OnAnimationShoot ||
        launchMode == ProjectileLaunchMode.OnAnimationShoot;

    public override bool IsLaunchedProjectile =>
        launchMode == ProjectileLaunchMode.Immediate ||
        launchMode == ProjectileLaunchMode.OnAnimationShoot;

    // Keep fl alive across composite HitTime destroy; ProjectileManager owns lifetime.
    public override bool OutlivesComposite => IsLaunchedProjectile;

    public override bool UsesCastHitLifetime => false;






    public bool ShouldLaunchImmediately => launchMode == ProjectileLaunchMode.Immediate;

    public bool ShouldLaunchOnShoot => launchMode == ProjectileLaunchMode.OnAnimationShoot;


}
