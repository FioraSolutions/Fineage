using System;
using UnityEngine;

public partial class WorldClock : MonoBehaviour
{
    public void SynchronizeClock(long gameTicks, int tickDurationMs, int dayDurationMinutes) {
        if (_startClock)
        {
            return;
        }

        float ticksPerDay = (float)dayDurationMinutes * 60 * 1000 / tickDurationMs;
        float currentHours = gameTicks / ticksPerDay * 24 % 24;
        this._dayDurationMinutes = dayDurationMinutes;
        float serverDayRatio = currentHours / 24f;
        _timeElapsed = serverDayRatio * dayDurationMinutes * 60f;
    }

    public bool IsNightTime() {
        return _clock.nightRatio > 0 && _clock.nightRatio <= 1 || _clock.dayRatio < 0.25f;
    }

    private void CalculateDayNightRatio() {
        if(_clock.totalRatio >= _worldTimer.dayStartTime && _clock.totalRatio < _worldTimer.dayEndTime) {
            _clock.nightRatio = 0;
            _clock.dayRatio = (_clock.totalRatio - _worldTimer.dayStartTime) / (_worldTimer.dayEndTime - _worldTimer.dayStartTime);
        } else {
            _clock.dayRatio = 0;
            if(_clock.totalRatio >= _worldTimer.dayEndTime) {
                _clock.nightRatio = (_clock.totalRatio - _worldTimer.dayEndTime) / (1.0f - _worldTimer.dayEndTime + _worldTimer.dayStartTime);
            } else {
                _clock.nightRatio = (_clock.totalRatio + (1.0f - _worldTimer.dayEndTime)) / (1.0f - _worldTimer.dayEndTime + _worldTimer.dayStartTime);
            }
        }
    }

    private void CalculateSunPhaseRatio() {
        _clock.dawnRatio = CalculatePeriodRatio(_worldTimer.sunriseStartTime, _worldTimer.sunriseEndTime);
        _clock.brightRatio = CalculatePeriodRatio(_worldTimer.sunriseEndTime, _worldTimer.sunsetStartTime);
        _clock.duskRatio = CalculatePeriodRatio(_worldTimer.sunsetStartTime, _worldTimer.sunsetEndTime);
        _clock.darkRatio = CalculatePeriodRatio(-.99f, _worldTimer.sunriseStartTime);
    }

    private float CalculatePeriodRatio(float startRatio, float endRatio) {
        float periodDuration = (startRatio < 0) ? (Mathf.Abs(startRatio) + endRatio) : (endRatio - startRatio);

        float ratio = 0;
        if(_clock.dayRatio >= 0 && _clock.nightRatio == 0) {
            // Day
            if(_clock.dayRatio <= endRatio) {
                // In Range        
                if(startRatio < 0) {
                    ratio += Mathf.Abs(startRatio);
                    ratio += _clock.dayRatio;
                } else if(_clock.dayRatio >= startRatio) {
                    ratio -= startRatio;
                    ratio += _clock.dayRatio;
                } else {
                    ratio = 0;
                }

                ratio = Mathf.Clamp(ratio / periodDuration, 0, 1);
            } else {
                ratio = 1;
            }
        } else {
            // Night
            if(startRatio < 0 && _clock.nightRatio > (1 + startRatio)) {
                ratio += Mathf.Clamp((_clock.nightRatio - (1 + startRatio)) / periodDuration, 0, 1);
            } else {
                ratio = 0;
            }
        }

        return ratio;
    }
}
