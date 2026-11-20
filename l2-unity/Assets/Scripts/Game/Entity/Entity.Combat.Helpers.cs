using System;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public partial class Entity
{
    public void UpdateNpcRunningSpd(float runSpeed)
    {
        if (runSpeed > 0.001f)
            _stats.RunRealSpeed = runSpeed;
        float scaled = UpdateRunSpeed(runSpeed);
        RefreshLocomotionAnimRates();
    }

    public virtual float UpdateSpeed(int speed) {
        float scaled = StatsConverterPacketServices.Converter.ConvertStat(Stat.SPEED, speed);
        _stats.Speed = speed;
        _stats.ScaledSpeed = scaled;
        return StatsConverterPacketServices.Converter.ConvertStat(Stat.SPEED, speed);
    }

    public virtual float UpdateRunSpeed(float speed)
    {
        if (speed <= 0.001f)
            return UnityRunSpeedMeters;
        float scaled = StatsConverterPacketServices.Converter.ConvertStat(Stat.SPEED, speed);
        _stats.RunRealSpeed = speed;
        _stats.UnitySpeedRun = scaled;
        return scaled;
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
        if (speed <= 0.001f)
            return UnityWalkSpeedMeters;
        float scaled = StatsConverterPacketServices.Converter.ConvertStat(Stat.SPEED, speed);
        _stats.WalkRealSpeed = speed;
        _stats.UnitySpeedWalking = scaled;
        return scaled;
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
