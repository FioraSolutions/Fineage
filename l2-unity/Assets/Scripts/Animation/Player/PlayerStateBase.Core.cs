using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public partial class PlayerStateBase : StateMachineBehaviour
{
    [SerializeField] protected bool _enabled = true;

    public void LoadComponents(Animator animator)
    {
        if (_entity == null)
        {
            _entity = animator.transform.parent.GetComponent<Entity>();
            if (_entity == null)
                _entity = animator.GetComponentInParent<Entity>();
        }

        if (_entity == null || _entity is UserEntity)
        {
            if (_entity == null)
            {
                FineageLog.Error(
                    FineageLogCategory.World,
                    "PlayerStateBase: no Entity on animator hierarchy animator=" + animator.name);
            }

            _enabled = false;
            return;
        }

        if (_gear == null)
            _gear = _entity.GetComponent<PlayerGear>();

        if (_audioHandler == null)
        {
            _audioHandler = animator.gameObject.GetComponent<CharacterAnimationAudioHandler>();
            if (_audioHandler == null)
                _audioHandler = animator.GetComponentInParent<CharacterAnimationAudioHandler>();
        }

        if (_animator == null)
            _animator = animator;
    }

    public void PlaySoundAtRatio(CharacterSoundEvent soundEvent, float ratio)
    {
        _audioHandler.PlaySoundAtRatio(soundEvent, ratio);
    }

    public void PlaySoundAtRatio(ItemSoundEvent soundEvent, float ratio)
    {
        _audioHandler.PlaySoundAtRatio(soundEvent, ratio);
    }

    protected void PlayEquippedWeaponItemSounds()
    {
        if (_gear == null)
        {
            int id = _entity != null && _entity.Identity != null ? _entity.Identity.Id : 0;
            FineageLog.Error(
                FineageLogCategory.World,
                "PlayerStateBase: PlayerGear missing for WeaponSwing entityId=" + id);
            return;
        }

        if (_audioHandler == null)
        {
            int id = _entity.Identity != null ? _entity.Identity.Id : 0;
            int weaponId = _gear.EquippedRightHandWeapon != null ? _gear.EquippedRightHandWeapon.Id : 0;
            string swing = CombatHitSoundResolver.ResolveWeaponSwingToken(weaponId);
            string shing = CombatHitSoundResolver.ResolveWeaponShingToken(weaponId);
            CharacterCombatAudioDispatch.PlayItemSoundToken(swing, _entity.transform.position, id);
            CharacterCombatAudioDispatch.PlayItemSoundToken(shing, _entity.transform.position, id);
            return;
        }

        int weaponId = _gear.EquippedRightHandWeapon != null ? _gear.EquippedRightHandWeapon.Id : 0;
        string swing = CombatHitSoundResolver.ResolveWeaponSwingToken(weaponId);
        string shing = CombatHitSoundResolver.ResolveWeaponShingToken(weaponId);
        _audioHandler.PlayItemSoundTokenAtRatio(swing, _audioHandler.SwishRatio);
        _audioHandler.PlayItemSoundTokenAtRatio(shing, _audioHandler.AtkRatio);
    }
}
