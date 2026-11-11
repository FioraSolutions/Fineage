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
}
