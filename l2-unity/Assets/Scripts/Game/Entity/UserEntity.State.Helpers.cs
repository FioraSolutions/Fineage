using VContainer;
using UnityEngine;

public partial class UserEntity : NetworkEntity
{
    public void ClearAttackVisual()
    {
        if (_attackVisualUntil > 0f)
            MagicGaitTrace.Event(this, "ClearAttackVisual", null);
        _attackVisualUntil = 0f;
    }

    public void PlayDeath()
    {
        SetDead(true);
        ClearAttackVisual();
    }

    public void EquipAllArmors()
    {
        PlayerAppearance appearance = _appearance as PlayerAppearance;
        UserGear gear = _gear as UserGear;
        if (appearance == null || gear == null)
        {
            // GearFlowLog.Warn("UserEntity.EquipAllArmors abort " + GearFlowLog.Entity(this) +
                // " appearance=" + (appearance != null) + " gear=" + (_gear != null ? _gear.GetType().Name : "null"));
            return;
        }

        // GearFlowLog.Info("UserEntity.EquipAllArmors " + GearFlowLog.Entity(this) +
            // " " + GearFlowLog.Paperdoll(appearance));
        gear.SyncEquippedArmor(appearance);
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        _networkAnimationReceive.SetAnimationProperty((int)PlayerAnimationEvent.death, 1f, true);
    }

    protected override void OnHit(bool criticalHit)
    {
        base.OnHit(criticalHit);
        if (_characterAnimationAudioHandler != null)
            _characterAnimationAudioHandler.PlaySound(CharacterSoundEvent.Dmg);
    }
}
