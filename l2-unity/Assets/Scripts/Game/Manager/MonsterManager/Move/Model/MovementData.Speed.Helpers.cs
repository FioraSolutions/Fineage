using UnityEngine;

public partial class MovementData
{
    public void SpeedAccum2d(float dist2d)
    {
        if (!_spdTripStarted)
        {
            _spdTripStarted = true;
            _spdTripStartTime = Time.time;
            _spdTripDist2d = 0f;
        }
        _spdTripDist2d += dist2d;
    }

    public bool SpeedShouldLog(float now, float intervalSec)
    {
        if (now - _spdLastLogTime < intervalSec)
            return false;
        _spdLastLogTime = now;
        return true;
    }

    public bool SpeedMarkGait(bool running, string gait)
    {
        if (!_spdHasGait)
        {
            _spdHasGait = true;
            _spdLastRunning = running;
            _spdLastGait = gait;
            return false;
        }
        bool changed = _spdLastRunning != running || _spdLastGait != gait;
        _spdLastRunning = running;
        _spdLastGait = gait;
        return changed;
    }

    public float SpeedTripSec(float now)
    {
        if (!_spdTripStarted)
            return 0f;
        return now - _spdTripStartTime;
    }

    public float SpeedTripDist2d()
    {
        return _spdTripDist2d;
    }
}
