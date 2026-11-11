using UnityEngine;
using VContainer;

public partial class UserEntity : NetworkEntity
{
    public void FaceTowards(Vector3 worldPoint)
    {
        Vector3 dir = worldPoint - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f)
            return;
        if (_pawnYaw == null)
            return;
        _pawnYaw.RequestLook(transform, worldPoint, L2PawnYaw.RotationRateYaw);
    }

    public void BeginAttackVisual(float durationSec)
    {
        float d = Mathf.Max(0.1f, durationSec);
        _attackVisualUntil = Time.time + d;
        MagicGaitTrace.Event(this, "BeginAttackVisual", "dur=" + d.ToString("F3"));
    }

    public float AttackVisualLeftSec()
    {
        if (_attackVisualUntil <= 0f)
            return -1f;
        return _attackVisualUntil - Time.time;
    }

    public bool IsAttackVisualPlaying()
    {
        return _attackVisualUntil > 0f && Time.time < _attackVisualUntil;
    }

    public bool ShouldReturnFromAttack()
    {
        return _attackVisualUntil > 0f && Time.time >= _attackVisualUntil;
    }

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
