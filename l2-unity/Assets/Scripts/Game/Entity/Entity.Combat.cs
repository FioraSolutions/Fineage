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

    /* Notify server that entity got attacked */
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

    /// <summary>DoT / StatusUpdate HP loss without an Attack packet (Mobius notifyDamageReceived).</summary>
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


    public void UpdateNpcRunningSpd(float runSpeed)
    {
        float scaled = StatsConverterPacketServices.Converter.ConvertStat(Stat.SPEED, runSpeed);
        float anim_converted = NpcLocomotionAnimRateForMesh("run");
        if (_networkAnimationReceive != null) _networkAnimationReceive.SetRunSpeed(anim_converted);
        Stats.UnitySpeedRun = scaled;

    }

    /// <summary>
    /// Monster/NPC locomotion anim rate read from AnimTween col "rate" for this
    /// mesh (runtimeAnimatorController.name = prefab stem, e.g. wolf → wolf_anim),
    /// corrected for the imported 24 fps Unity clip timeline: 1.10 * rate/24.
    /// Falls back to the former 1.10 * 30/24 hardcode when the mesh is not listed.
    /// </summary>
    float NpcLocomotionAnimRateForMesh(string sequence)
    {
        string controllerName = _networkAnimationReceive != null
            ? _networkAnimationReceive.GetAnimatorName()
            : null;

        if (string.IsNullOrEmpty(controllerName))
            return NumberUtils.NpcLocomotionAnimRate();

        // runtimeAnimatorController.name is the override controller's stem
        // (e.g. "wolf"); AnimTween lists it as "<stem>_anim" (e.g. wolf_anim).
        string mesh = controllerName.EndsWith("_anim", System.StringComparison.OrdinalIgnoreCase)
            ? controllerName
            : controllerName + "_anim";

        float rate = TablePacketServices.AnimTweens.TryGetRate(mesh, sequence);
        return NumberUtils.NpcLocomotionAnimRate(rate);
    } 



    public virtual float UpdateSpeed(int speed) {
        float scaled = StatsConverterPacketServices.Converter.ConvertStat(Stat.SPEED, speed);
        _stats.Speed = speed;
        _stats.ScaledSpeed = scaled;
        return StatsConverterPacketServices.Converter.ConvertStat(Stat.SPEED, speed);
    }

    public virtual float UpdateRunSpeed(float speed)
    {
        float scaled = StatsConverterPacketServices.Converter.ConvertStat(Stat.SPEED, speed);
        _stats.UnitySpeedRun = speed;
        return scaled;
        //_stats.ScaledRunSpeed = scaled;
        //return StatsConverterPacketServices.Converter.ConvertStat(Stat.SPEED, speed);
    }

    public virtual float UpdateAnimRunSpeed(float speed)
    {
        return StatsConverterPacketServices.Converter.ConvertStat(Stat.ANIM_RUN_SPEED, speed);
    }

    public virtual float UpdateAnimWalkSpeed(float speed)
    {
        return StatsConverterPacketServices.Converter.ConvertStat(Stat.ANIM_WALK_SPEED, speed);
    }


    public float UpdateAnimRunMagicSpeed(float speed)
    {
        return StatsConverterPacketServices.Converter.ConvertStat(Stat.ANIM_MAGIC_RUN_SPEED, speed);
    }

    public virtual float UpdateWalkSpeed(float speed)
    {
        float scaled = StatsConverterPacketServices.Converter.ConvertStat(Stat.SPEED, speed);
        _stats.UnitySpeedWalking = scaled;
        return scaled;
        //_stats.ScaledWalkSpeed = scaled;
       // return StatsConverterPacketServices.Converter.ConvertStat(Stat.SPEED, speed);
    }
   
    public virtual bool IsDead() {
        if (_dead) return true;
        return _status != null && _status.GetHp() <= 0;
    }



    public double Hp()
    {
        return _status.GetHp();
    }

    

    public virtual void UpdateWaitType(WaitType moveType)
    {

    }

    public virtual void UpdateMoveType(bool running)
    {
        Running = running;
    }

}
