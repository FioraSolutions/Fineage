using System;
using UnityEngine;

public partial class WorldClock
{
    void OnDisable()
    {
        if (_persistentDriver)
        {
            return;
        }

        if (_instance == this)
        {
            _instance = null;
        }
    }

    void OnDestroy() {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    void Update() {
        if (!_persistentDriver && Application.isPlaying && _instance != null && _instance != this)
        {
            return;
        }

        if (_mobiusServerClock)
        {
            TickMobiusServerClock();
            CalculateDayNightRatio();
            CalculateSunPhaseRatio();
            return;
        }

        if (Application.isPlaying && _persistentDriver)
        {
            LogAwaitingServerGameTimeOnce();
            if (_holdAtHour)
                SetWorldHours(_heldHour);
            CalculateDayNightRatio();
            CalculateSunPhaseRatio();
            return;
        }

        if (_holdAtHour)
        {
            SetWorldHours(_heldHour);
            if (Application.isPlaying && !_loggedHold)
            {
                _loggedHold = true;
            }
            CalculateDayNightRatio();
            CalculateSunPhaseRatio();
            return;
        }

        if (Application.isPlaying && !_loggedHold)
        {
            _loggedHold = true;
        }

        if (_startClock)
            UpdateClock();

        CalculateDayNightRatio();
        CalculateSunPhaseRatio();
    }
}
