using System;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public partial class Entity
{
    public float GetCollissionRadius()
    {
        return VectorUtils.ConvertL2jDistance(_appearance.CollisionRadius);
    }

    public Entity GetTargetEntity()
    {
        if (_target == null)
        {
            _lastTarget = null;
            _cachedTargetEntity = null;
            return null;
        }

        if (_cachedTargetEntity == null || _lastTarget != _target)
        {
            _lastTarget = _target;
            _cachedTargetEntity = _target.GetComponent<Entity>();
        }

        return _cachedTargetEntity;
    }

    public void SetDamage(int damage) 
    {
        _status.SetDamage(damage);
    }

    public void SetSelfHit(Hit hit)
    {
        _selfHit = hit;
    }

    public bool HitIsMissed()
    {
        if (_selfHit == null) return false;
        return _selfHit.isMiss();
    }

    public double CalculateRemainingHp()
    {
        return _status.GetRemainingHp();
    }

    public void SetupTotalCastDuration(float serverHitTimeMs, float flightTimeMs, float[] clipsDurations, float shotEventTime, int targetObjectId = 0, int castStyle = 1, bool mixCombo = false)
    {
        if (_castData == null) _castData = new MagicCastData();

        _castData.Setup(serverHitTimeMs, flightTimeMs, clipsDurations, shotEventTime, targetObjectId, castStyle, mixCombo);
    }

    public MagicCastData GetMagicCastData()
    {
        return _castData;
    }
}
