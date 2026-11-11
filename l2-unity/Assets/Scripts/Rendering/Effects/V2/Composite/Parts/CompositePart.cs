using UnityEngine;

[System.Serializable]
public abstract partial class CompositePart : ISerializationCallbackReceiver
{
    public string name;
    public BaseEffect prefab;
    public float scale = 1f;
    public CompositePartSpawnTiming spawnTiming = CompositePartSpawnTiming.Immediate;
    public float spawnDelaySeconds;
    [Tooltip("Where this part appears. Spawn Timing is when.")]
    public EffectAttachmentPoint spawnAttachmentPoint;
    public string boneName;
    public bool follow = true;
    public bool inheritRotation;
    // Nested SerializeReference inside a SerializeReference part orphans RefIds
    // (missing 1008/1009) on prefab save. Runtime uses spawnAttachmentPoint.
    // Kept only to migrate old prefabs in OnAfterDeserialize.
    [HideInInspector]
    [SerializeReference] public EffectPlacement placement;

    public bool IsSpawnable => prefab != null;

    public virtual bool WantsAnimationShoot =>
        spawnTiming == CompositePartSpawnTiming.OnAnimationShoot;

    public virtual bool WantsHitCollider =>
        spawnTiming == CompositePartSpawnTiming.OnHitCollider;

    public virtual bool IsLaunchedProjectile => false;

    /// <summary>
    /// Independent burst (skill _ta): plays out after the composite GO is gone.
    /// Generic weapon/bow hit FX are separate; this is the skill overlay.
    /// </summary>
    public virtual bool OutlivesComposite => false;

    /// <summary>
    /// Stationary cast FX stretch GO life to server HitTime. Shot / impact do not.
    /// </summary>
    public virtual bool UsesCastHitLifetime =>
        spawnTiming != CompositePartSpawnTiming.OnHitCollider;

    /// <summary>
    /// AttachToBone: keep the weapon socket rotation so Relative CS +X is the blade.
    /// </summary>
    public virtual bool UsesWeaponBoneRotation => false;

    /// <summary>
    /// Part-owned playback policy. ParticleGroupV2 only receives a window + stop mode.
    /// </summary>






}
