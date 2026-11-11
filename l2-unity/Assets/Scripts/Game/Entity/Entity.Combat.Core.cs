using System;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public partial class Entity
{
    public void ApplyDamage(int damage, bool criticalHit) {
        if(_status.GetHp() <= 0) {
            // Debug.LogWarning("Trying to apply damage to a dead entity");
            return;
        }

        _status.SetHp(Mathf.Max((float)_status.GetHp() - damage, 0));

        OnHit(criticalHit);

        if(_status.GetHp() <= 0) {
            OnDeath();
        }
    }

    protected virtual void EquipAllWeapons() {
        if(_gear == null) {
            // Debug.LogWarning("Gear script is not attached to entity");
            return;
        }
        _gear.SyncEquippedWeapons(_appearance.RHand, _appearance.LHand);
    }

    public void InflictAttack(AttackType attackType) {
    }

    protected virtual void OnDeath() {
        if(_networkAnimationReceive != null) {
            _networkAnimationReceive.enabled = false;
        }
    }

    protected virtual void OnHit(bool criticalHit) {
        // TODO: Add armor type for more hit sounds
        if (!HitImpactSound.Play(criticalHit) && AudioPacketServices.Audio != null)
            AudioPacketServices.Audio.PlayHitSound(criticalHit, transform.position);
    }

    public void PlayHitReaction(bool criticalHit = false) => OnHit(criticalHit);

    public virtual void OnStopMoving() {}

    public virtual void OnStartMoving(bool walking) {}

    public virtual float UpdatePAtkSpeed(int pAtkSpd) {
        _stats.PAtkSpd = pAtkSpd;
        //float convert =  StatsConverterPacketServices.Converter.ConvertStat(Stat.PHYS_ATTACK_SPEED, pAtkSpd);
        return  StatsConverterPacketServices.Converter.ConvertStat(Stat.SPEED, pAtkSpd);
    }

    public virtual float UpdateMAtkSpeed(int mAtkSpd) {
        _stats.MAtkSpd = mAtkSpd;
        return StatsConverterPacketServices.Converter.ConvertStat(Stat.MAGIC_ATTACK_SPEED, mAtkSpd);
    }

    public void UpdateNpcPAtkSpd(int pAtkSpd)
    {
        if (pAtkSpd == 0) return;
        float timeAtck = CalcBaseParam.CalculateTimeL2j(pAtkSpd);
        float speedAnim = CalcBaseParam.GetAnimatedSpeed(pAtkSpd, timeAtck);
        _networkAnimationReceive.SetPAtkSpd(speedAnim);
    }

    public void UpdateNpcWalkSpd(float walkSpeed)
    {
        float scaled = StatsConverterPacketServices.Converter.ConvertStat(Stat.SPEED, walkSpeed);
        float anim_converted = NpcLocomotionAnimRateForMesh("Walk");
        if (_networkAnimationReceive != null) _networkAnimationReceive.SetWalkSpeed(anim_converted);
        Stats.UnitySpeedWalking = scaled;
    }
}
