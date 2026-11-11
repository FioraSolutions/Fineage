public partial class L2WeaponChargeGroup : BaseEffectGroup, IWeaponEffect
{
    private Transform _weaponTransform;
    private Transform _swordTip;
    private float adjustedLife;
    private const float ReferenceWeaponLength = 0.3550376f;
    private float _timeMultiplier = 1;
    private static readonly int StartTimeID = Shader.PropertyToID("_StartTime");
    private static readonly int SeedID = Shader.PropertyToID("_Seed");
    private static readonly int LifetimeRangeID = Shader.PropertyToID("_LifetimeRange");
    private static readonly int FadeoutStartTimeID = Shader.PropertyToID("_FadeoutStartTime");
    private static readonly int InitialDelayRangeID = Shader.PropertyToID("_InitialDelayRange");


    private MaterialPropertyBlock _propBlock;






}
