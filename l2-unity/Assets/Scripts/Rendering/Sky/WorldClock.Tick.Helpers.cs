using System;
using UnityEngine;

public partial class WorldClock
{
    private void Awake()
    {
        ClaimInstance();
    }

    void OnEnable()
    {
        if (Application.isPlaying)
        {
            _startClock = true;
            if (_dayDurationMinutes < 0.5f || _dayDurationMinutes > 5f)
            {
                _dayDurationMinutes = 2.5f;
            }
        }

        ClaimInstance();
    }

    void ClaimInstance()
    {
        if (_bootstrapping || _persistentDriver)
        {
            _persistentDriver = true;
            _instance = this;
            WorldClockPacketServices.Bind(clock: this);
            return;
        }

        if (!Application.isPlaying)
        {
            if (_instance == null)
            {
                _instance = this;
                WorldClockPacketServices.Bind(clock: this);
            }

            return;
        }

        EnsurePersistent();
    }
}
