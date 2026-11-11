using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class AmbientSoundEmitter : EventHandler
{
    private void Awake() {
        if(!_eventDescription.isValid()) {
            Lookup();
        }

        int lengthMs = 0;
        _eventDescription.getLength(out lengthMs);
        _clipLengthSeconds = lengthMs / 1000f;
        _clipLengthSeconds = _clipLengthSeconds / _soundPitch;

        _eventDescription.is3D(out _isSound3D);
    }

    private void Lookup() {
        _eventDescription = RuntimeManager.GetEventDescription(_eventReference);
    }
}
