using VContainer;
using UnityEngine;

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

        NetworkAnimationController nac = GetAnimatorController();
        if (nac != null)
            PlayerLocomotionBodyCatalog.ValidateWaitHandOverride(
                RaceId,
                nac.GetAnimator(),
                Identity != null ? Identity.Id : 0);

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
        EquipAllWeapons();
        EquipAllArmors();
        UserGear userGear = _gear as UserGear;
        if (userGear != null)
            userGear.SyncArmorEnchantGlows();
        if (_gear != null)
            _gear.SyncEnchantGlows();
    }

    public override float UpdateRunSpeed(float speed)
    {
        float converted = base.UpdateRunSpeed(speed);
        RefreshLocomotionAnimRates();
        return converted;
    }

    public override float UpdateWalkSpeed(float speed)
    {
        float converted = base.UpdateWalkSpeed(speed);
        RefreshLocomotionAnimRates();
        return converted;
    }
}