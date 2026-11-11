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
}
