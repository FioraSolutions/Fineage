using System;
using UnityEngine;

[System.Serializable]
public struct WorldTimer {
    public float dayStartTime; //0.25f
    public float dayEndTime; //0.75f
    public float sunriseStartTime; //0f
    public float sunriseEndTime; //0.15f
    public float sunsetStartTime; //0.85f
    public float sunsetEndTime; //0.99f
}

[System.Serializable]
public struct Clock {
    public float totalRatio;
    [Header("Day/Night cycle")]
    public float dayRatio;
    public float nightRatio;
    [Header("Full day cycles")]
    public float dawnRatio;
    public float brightRatio;
    public float duskRatio;
    public float darkRatio;
}

[ExecuteInEditMode]
public partial class WorldClock : MonoBehaviour {
{
    [SerializeField] private float _dayDurationMinutes = 2.5f;
    [SerializeField] private string _timeHour;
    [SerializeField] private float _timeElapsed = 0;
    [SerializeField] private bool _startClock = true;
    [SerializeField] private bool _holdAtHour = true;
    [SerializeField] private float _heldHour = 13f;
    [SerializeField] private WorldTimer _worldTimer;
    [SerializeField] private Clock _clock;

    public Clock Clock { get { return _clock; } }

    /// <summary>L2 world hour in [0, 24). Maps Clock.totalRatio * 24 (midnight = 0).</summary>
    public float WorldHours
    {
        get { return _clock.totalRatio * 24f; }
    }

    private static WorldClock _instance;
    [System.Obsolete("Use [Inject] or WorldClockPacketServices.Clock instead")]
    public static WorldClock Instance { get { return _instance; } }

    bool _persistentDriver;
    static bool _bootstrapping;
    static bool _loggedHold;


}
