using System.Collections.Generic;
using UnityEngine;

public class UserStateBase : StateMachineBehaviour {
    protected CharacterAnimationAudioHandler _audioHandler;
    protected NetworkAnimationController _networkAnimationController;
    protected Animator _animator;
    protected Entity _entity;
    protected UserGear _gear;
    protected bool _cancelAction = false;
    [SerializeField] protected bool _enabled = true;
    [SerializeField] protected string _weaponAnim;

    public void LoadComponents(Animator animator) {
        if (_entity == null) {
            if(animator.transform.parent == null) {
                _enabled = false;
                return;
            } else if (animator.transform.parent.parent == null) {
                _enabled = false;
                return;
            }
            _entity = animator.transform.parent.parent.GetComponent<Entity>();
        }
        if (_entity == null || _entity is PlayerEntity) {
            _enabled = false;
            return;
        }
        if (_gear == null) {
            _gear = _entity.GetComponent<UserGear>();
        }

        if (_gear == null)
        {
            FineageLog.Error(FineageLogCategory.World, "LocomotionProfile: remote UserGear missing on entity");
            _weaponAnim = LocomotionProfile.BareHand.AnimSuffix;
        }
        else
        {
            _weaponAnim = LocomotionProfileResolver.RemoteAnimatorSuffix(
                _gear.CombatStyle,
                _gear.gameObject.name);
        }

        if (_audioHandler == null) {
            _audioHandler = animator.gameObject.GetComponent<CharacterAnimationAudioHandler>();
        }
        if (_animator == null) {
            _animator = animator;
        }
        if (_networkAnimationController == null) {
            _networkAnimationController = _entity.transform.GetComponent<NetworkAnimationController>();
        }
    }

    public void PlaySoundAtRatio(CharacterSoundEvent soundEvent, float ratio) {
        _audioHandler.PlaySoundAtRatio(soundEvent, ratio);
    }

    public void PlaySoundAtRatio(ItemSoundEvent soundEvent, float ratio) {
        _audioHandler.PlaySoundAtRatio(soundEvent, ratio);
    }

    protected void PlayEquippedWeaponItemSounds() {
        if (_audioHandler == null || _gear == null)
            return;

        int weaponId = _gear.EquippedRightHandWeapon != null ? _gear.EquippedRightHandWeapon.Id : 0;
        string swing = CombatHitSoundResolver.ResolveWeaponSwingToken(weaponId);
        string shing = CombatHitSoundResolver.ResolveWeaponShingToken(weaponId);
        _audioHandler.PlayItemSoundTokenAtRatio(swing, _audioHandler.SwishRatio);
        _audioHandler.PlayItemSoundTokenAtRatio(shing, _audioHandler.AtkRatio);
    }

    public void SetBool(string name, bool value) {
        _cancelAction = true;
        AnimatorBoolSet.SetIfExists(_animator, name, value);
    }
}
