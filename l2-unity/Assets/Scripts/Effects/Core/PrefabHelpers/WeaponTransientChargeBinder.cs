using UnityEngine;

/// <summary>
/// Stick charge (soul_N_stick): parent to the weapon, Relative CS, local +X travel.
/// If the blade is longer than 12 UU, overwrite emitter indices 0 and 1
/// StartVelocityRange.X and aim +X from Sword_Base to Sword_Tip.
/// Bows play soul_N_bow instead of this prefab.
/// </summary>
[DisallowMultipleComponent]
public sealed partial class WeaponTransientChargeBinder : MonoBehaviour, IEffectPrePlayConfigurator
{
    public const float UnityMetersToUu = 52.5f;
    public const float ExtraOverwriteThresholdUu = 12f;
    public const int MaxTravelingEmitters = 2;
    const string SwordTipName = "Sword_Tip";
    const string SwordBaseName = "Sword_Base";

    [Tooltip("Native SpawnNTransientEffect patches emitter indices 0 and 1.")]
    [SerializeField] ParticleGroupV2[] _travelingEmitters;
    [SerializeField] bool _debugLogs;

    Transform _weapon;
    Vector4[] _authoredVelocityX;
    bool[] _hasAuthoredVelocityX;

}
