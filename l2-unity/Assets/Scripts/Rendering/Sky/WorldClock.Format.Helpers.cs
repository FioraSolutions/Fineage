using System;
using UnityEngine;

public partial class WorldClock
{
    private void UpdateClock() {
        _timeElapsed += Time.deltaTime;

        if(_timeElapsed >= (_dayDurationMinutes * 60f)) {
            _timeElapsed = 0;
        }

        _clock.totalRatio = _timeElapsed / (_dayDurationMinutes * 60f);

        // Calculate the number of seconds based on the percentage
        int seconds = (int)(_clock.totalRatio * 86400);

        // Convert seconds to hours, minutes, and seconds
        int hours = seconds / 3600;
        int minutes = (seconds % 3600) / 60;
        int remainingSeconds = seconds % 60;

        // Create a TimeSpan object with the calculated hours, minutes, and seconds
        TimeSpan time = new TimeSpan(hours, minutes, remainingSeconds);

        // Format the TimeSpan object as a string in the desired format (HH:mm:ss)
        _timeHour = time.ToString(@"hh\:mm\:ss");
    }

    void PinAtHour(float hours)
    {
        _holdAtHour = true;
        _heldHour = hours;
        SetWorldHours(hours);
    }

    public void SetWorldHours(float hours)
    {
        float t = hours % 24f;
        if (t < 0f)
        {
            t += 24f;
        }

        _clock.totalRatio = t / 24f;
        _timeElapsed = _clock.totalRatio * _dayDurationMinutes * 60f;
        int seconds = (int)(_clock.totalRatio * 86400);
        int h = seconds / 3600;
        int m = (seconds % 3600) / 60;
        int s = seconds % 60;
        _timeHour = new TimeSpan(h, m, s).ToString(@"hh\:mm\:ss");
        CalculateDayNightRatio();
        CalculateSunPhaseRatio();
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
