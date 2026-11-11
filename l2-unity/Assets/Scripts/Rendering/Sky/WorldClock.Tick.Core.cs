using System;
using UnityEngine;

public partial class WorldClock
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics()
    {
        _instance = null;
        _bootstrapping = false;
        _loggedHold = false;
    }

    public static WorldClock EnsurePersistent()
    {
        if (_instance != null && _instance._persistentDriver)
        {
            _instance.enabled = true;
            _instance.gameObject.SetActive(true);
            _instance._startClock = true;
            _instance._dayDurationMinutes = 2.5f;
            _instance.PinAtHour(13f);
            return _instance;
        }

        // DayNightCycle is ExecuteInEditMode — never create DDOL drivers in edit mode.
        if (!Application.isPlaying)
        {
            if (_instance != null)
                return _instance;
            return FindFirstObjectByType<WorldClock>(FindObjectsInactive.Include);
        }

        if (_bootstrapping)
        {
            return _instance;
        }

        WorldClock existing = _instance;
        if (existing == null)
        {
            existing = FindFirstObjectByType<WorldClock>(FindObjectsInactive.Include);
        }

        _bootstrapping = true;
        var go = new GameObject("L2ClockDriver");
        DontDestroyOnLoad(go);
        WorldClock driver = go.AddComponent<WorldClock>();
        driver._persistentDriver = true;
        driver._startClock = true;
        driver._dayDurationMinutes = 2.5f;
        if (existing != null && existing != driver)
        {
            driver._worldTimer = existing._worldTimer;
        }

        driver.PinAtHour(13f);

        _instance = driver;
        WorldClockPacketServices.Bind(clock: driver);
        _bootstrapping = false;
        // Debug.Log("[L2Clock] persistent driver started (survives Menu unload / map tiles)");
        return driver;
    }
}
