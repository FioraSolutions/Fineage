using System;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public partial class Entity
{
    public void UpdateNpcRunningSpd(float runSpeed)
    {
        float scaled = StatsConverterPacketServices.Converter.ConvertStat(Stat.SPEED, runSpeed);
        float anim_converted = NpcLocomotionAnimRateForMesh("run");
        if (_networkAnimationReceive != null) _networkAnimationReceive.SetRunSpeed(anim_converted);
        Stats.UnitySpeedRun = scaled;

    }

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
