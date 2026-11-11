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
}
