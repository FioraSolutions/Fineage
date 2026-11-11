using UnityEngine;
using VContainer;

public partial class UserEntity : NetworkEntity
{
    [Inject] L2PawnYaw _pawnYaw;

    public override void Initialize()
    {
        base.Initialize();
        _characterController = GetComponent<CharacterController>();
        if (transform.childCount > 0)
            _characterAnimationAudioHandler = transform.GetChild(0).GetComponentInChildren<CharacterAnimationAudioHandler>();

        EquipAllArmors();

        EntityLoaded = true;
    }

    public CharacterController GetCharacterController()
    {
        if (_characterController == null)
            _characterController = GetComponent<CharacterController>();
        return _characterController;
    }

    public void RefreshVisuals()
    {
        // GearFlowLog.Info("UserEntity.RefreshVisuals nick=" + Nick +
            // " id=" + (Identity != null ? Identity.Id : 0) +
            // " " + GearFlowLog.Paperdoll(this) +
            // " gear=" + (_gear != null ? _gear.GetType().Name : "null"));
        EquipAllWeapons();
        EquipAllArmors();
    }

    public override float UpdateRunSpeed(float speed)
    {
        float converted = base.UpdateRunSpeed(speed);
        if (Stats != null)
            Stats.UnitySpeedRun = converted;
        float anim = ApplyAnimRunSpeed(speed);
        // Debug.Log(LogTag + " UpdateRunSpeed id=" +
            // (Identity != null ? Identity.Id : 0) +
            // " l2=" + speed + " unity=" + converted +
            // " anim=" + anim);
        return converted;
    }

    public override float UpdateWalkSpeed(float speed)
    {
        float converted = base.UpdateWalkSpeed(speed);
        ApplyAnimWalkSpeed(speed);
        return converted;
    }

    float ApplyAnimRunSpeed(float serverValue)
    {
        if (_networkAnimationReceive == null)
            return 0f;

        PlayerAppearance appearance = _appearance as PlayerAppearance;
        bool twoHanded = _gear != null && _gear.IsTwoHandedEquipped();
        float anim = appearance != null
            ? CharTemplateRegistry.GetRunSpeed(appearance.BaseClass, appearance.Sex, serverValue, twoHanded)
            : UpdateAnimRunSpeed(serverValue);
        _networkAnimationReceive.SetRunSpeed(anim);
        return anim;
    }

    void ApplyAnimWalkSpeed(float serverValue)
    {
        if (_networkAnimationReceive == null)
            return;

        PlayerAppearance appearance = _appearance as PlayerAppearance;
        bool twoHanded = _gear != null && _gear.IsTwoHandedEquipped();
        float anim = appearance != null
            ? CharTemplateRegistry.GetWalkSpeed(appearance.BaseClass, appearance.Sex, serverValue, twoHanded)
            : UpdateAnimWalkSpeed(serverValue);
        _networkAnimationReceive.SetWalkSpeed(anim);
    }

    public void LogSpeed(string where)
    {
        Stats s = Stats;
        float move = 0f;
        if (s != null)
            move = Running ? s.UnitySpeedRun : s.UnitySpeedWalking;
        string msg = LogTag + " speed " + where +
            " id=" + (Identity != null ? Identity.Id : 0) +
            " running=" + Running +
            " BaseRun=" + (s != null ? s.BaseRunSpeed : 0) +
            " BaseWalk=" + (s != null ? s.BaseWalkingSpeed : 0) +
            " RunReal=" + (s != null ? s.RunRealSpeed : 0f) +
            " WalkReal=" + (s != null ? s.WalkRealSpeed : 0f) +
            " unityRun=" + (s != null ? s.UnitySpeedRun : 0f) +
            " unityWalk=" + (s != null ? s.UnitySpeedWalking : 0f) +
            " moveSpeed=" + move;
        if (move <= 0.001f)
            Debug.LogWarning(msg);
        // else
            // Debug.Log(msg);
    }

    public void BindCharInfoAnimEvents(UserBowArrowEvents bowArrowEvents)
    {
        _bowArrowEvents?.Exit();
        if (Identity == null)
            return;
        _bowArrowEvents = bowArrowEvents;
        _bowArrowEvents?.Enter();
    }

    void OnDestroy()
    {
        _bowArrowEvents?.Exit();
        _bowArrowEvents = null;
    }
}
