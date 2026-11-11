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

        if (_holdAtHour)
        {
            SetWorldHours(_heldHour);
            if (Application.isPlaying && !_loggedHold)
            {
                _loggedHold = true;
                // Debug.Log("[L2Clock] sun held at " + _heldHour.ToString("0.###") + ":00");
            }
            return;
        }

        if (Application.isPlaying && !_loggedHold)
        {
            _loggedHold = true;
            // Debug.Log("[L2Clock] day runs in " + _dayDurationMinutes.ToString("0.###") + " min");
        }

        if(_startClock) {
            UpdateClock();
        }

        CalculateDayNightRatio();
        CalculateSunPhaseRatio();
    }
}
