using UnityEngine;

public sealed partial class HomeProjectileMover
{
    Vector3 ComputeNextPosition(Vector3 current, Vector3 target)
    {
        _speed += _acceleration * Time.deltaTime;
        if (_maxSpeed > 0f)
        {
            _speed = Mathf.Min(_speed, _maxSpeed);
        }

        return _path.Step(
            _startPosition,
            current,
            target,
            _speed,
            Time.deltaTime,
            ref _traveledDistance,
            ref _pathDistanceEstimate,
            out _pathCompleted);
    }

    void SyncOwnerWorldPosOverride()
    {
        if (_effectParts == null)
        {
            return;
        }

        Vector3 worldPosition = transform.position;
        for (int i = 0; i < _effectParts.Length; i++)
        {
            EffectPart part = _effectParts[i];
            if (part != null)
            {
                part.SetOwnerWorldPosOverride(true, worldPosition);
            }
        }
    }

    void ClearOwnerWorldPosOverride()
    {
        if (_effectParts == null)
        {
            return;
        }

        for (int i = 0; i < _effectParts.Length; i++)
        {
            EffectPart part = _effectParts[i];
            if (part != null)
            {
                part.SetOwnerWorldPosOverride(false, Vector3.zero);
            }
        }
    }
}
