using System;
using UnityEngine;

public partial class WorldClock
{
    const float DefaultMobiusClockSpeed = 6f;

    bool _mobiusServerClock;
    bool _loggedAwaitingServerTime;
    int _mobiusGameMinutesAnchor;
    float _mobiusSyncUnscaledTime;
    float _mobiusRealSecondsPerGameMinute = 10f;

    public event Action<int, int> GameTimeLabelChanged;

    public bool HasMobiusServerClock => _mobiusServerClock;

    public bool TryGetMobiusGameHourMinute(out int hour24, out int minute)
    {
        hour24 = 0;
        minute = 0;
        if (!_mobiusServerClock)
            return false;

        float total = CurrentMobiusGameMinutes();
        int totalMinutes = Mathf.FloorToInt(total) % InterludeMinimapGameTime.MinutesPerGameDay;
        if (totalMinutes < 0)
            totalMinutes += InterludeMinimapGameTime.MinutesPerGameDay;
        hour24 = totalMinutes / 60;
        minute = totalMinutes % 60;
        return true;
    }

    public void ApplyMobiusClientSetTime(int gameMinutes, int clockSpeed)
    {
        if (clockSpeed <= 0)
        {
            Debug.LogError("[GameTime] ClientSetTime rejected: clock speed must be positive (got " + clockSpeed + ").");
            return;
        }

        if (gameMinutes < 0 || gameMinutes >= InterludeMinimapGameTime.MinutesPerGameDay)
        {
            Debug.LogError(
                "[GameTime] ClientSetTime game minutes out of range: " + gameMinutes +
                "; normalizing modulo " + InterludeMinimapGameTime.MinutesPerGameDay + ".");
            gameMinutes = ((gameMinutes % InterludeMinimapGameTime.MinutesPerGameDay) +
                           InterludeMinimapGameTime.MinutesPerGameDay) %
                          InterludeMinimapGameTime.MinutesPerGameDay;
        }

        if (clockSpeed != (int)DefaultMobiusClockSpeed)
        {
            Debug.LogError(
                "[GameTime] ClientSetTime clock speed " + clockSpeed +
                " differs from Mobius Interlude default " + (int)DefaultMobiusClockSpeed +
                "; using server value for advance rate.");
        }

        _mobiusServerClock = true;
        _holdAtHour = false;
        _loggedAwaitingServerTime = false;
        _mobiusGameMinutesAnchor = gameMinutes;
        _mobiusSyncUnscaledTime = Time.unscaledTime;
        _mobiusRealSecondsPerGameMinute = 60f / clockSpeed;
        _dayDurationMinutes = InterludeMinimapGameTime.MinutesPerGameDay * _mobiusRealSecondsPerGameMinute / 60f;
        _startClock = true;
        PushMobiusGameTimeFromMinutes(CurrentMobiusGameMinutes());
    }

    float CurrentMobiusGameMinutes()
    {
        float elapsed = Time.unscaledTime - _mobiusSyncUnscaledTime;
        float advanced = elapsed / _mobiusRealSecondsPerGameMinute;
        float total = _mobiusGameMinutesAnchor + advanced;
        total %= InterludeMinimapGameTime.MinutesPerGameDay;
        if (total < 0f)
            total += InterludeMinimapGameTime.MinutesPerGameDay;
        return total;
    }

    void TickMobiusServerClock()
    {
        if (!_mobiusServerClock)
            return;

        PushMobiusGameTimeFromMinutes(CurrentMobiusGameMinutes());
    }

    void PushMobiusGameTimeFromMinutes(float totalMinutes)
    {
        SetWorldHours(totalMinutes / 60f);
        int hour = Mathf.FloorToInt(totalMinutes / 60f) % 24;
        int minute = Mathf.FloorToInt(totalMinutes) % 60;
        GameTimeLabelChanged?.Invoke(hour, minute);
    }

    void LogAwaitingServerGameTimeOnce()
    {
        if (_loggedAwaitingServerTime || !Application.isPlaying || _mobiusServerClock)
            return;

        _loggedAwaitingServerTime = true;
        Debug.LogError(
            "[GameTime] No ClientSetTime from zone yet; minimap clock and day/night stay frozen until server sync.");
    }
}