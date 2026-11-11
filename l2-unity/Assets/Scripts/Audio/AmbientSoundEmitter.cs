using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class AmbientSoundEmitter : EventHandler
{
    private FMOD.Studio.EventDescription _eventDescription;
    private FMOD.Studio.EventInstance _instance;

    [SerializeField] private EventReference _eventReference;
    [SerializeField] private EmitterGameEvent _playEvent = EmitterGameEvent.None;
    [SerializeField] private EmitterGameEvent _stopEvent = EmitterGameEvent.None;
    [SerializeField] private AmbientSoundType _ambientSoundType;
    [SerializeField] private bool _allowFadeout = true;
    [SerializeField] private bool _overrideAttenuation = false;
    [SerializeField] private float _loopDelaySeconds = 1;
    [SerializeField] private float _playChancePercent = 100;
    [SerializeField] private float _soundPitch = 0;
    [SerializeField] private bool _loop = true;
    [SerializeField] private float _overrideMinDistance = -1.0f;
    [SerializeField] private float _overrideMaxDistance = -1.0f;
    [SerializeField] private float _clipLengthSeconds = 0;
    [SerializeField] private bool _isSound3D = true;

    public EventReference EventReference { set { _eventReference = value; } }
    public EmitterGameEvent PlayEvent { set { _playEvent = value; } }
    public EmitterGameEvent StopEvent { set { _stopEvent = value; } }
    public AmbientSoundType AmbientSoundType { set { _ambientSoundType = value; } }
    public bool AllowFadeout { set { _allowFadeout = value; } }
    public bool OverrideAttenuation { set { _overrideAttenuation = value; } }
    public float LoopDelaySeconds { set { _loopDelaySeconds = value; } }
    public float PlayChancePercent { set { _playChancePercent = value; } }
    public float SoundPitch { set { _soundPitch = value; } }
    public bool Loop { set { _loop = value; } }
    public float OverrideMinDistance { set { _overrideMinDistance = value; } }
    public float OverrideMaxDistance { set { _overrideMaxDistance = value; } }

}
