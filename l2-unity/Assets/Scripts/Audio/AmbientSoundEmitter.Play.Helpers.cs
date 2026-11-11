using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class AmbientSoundEmitter : EventHandler
{
    private void StopInstance() {
        if(_instance.isValid()) {
            _instance.stop(_allowFadeout ? FMOD.Studio.STOP_MODE.ALLOWFADEOUT : FMOD.Studio.STOP_MODE.IMMEDIATE);
            _instance.release();
            if(!_allowFadeout) {
                _instance.clearHandle();
            }
        }
    }

    public bool IsPlaying() {
        if(_instance.isValid()) {
            FMOD.Studio.PLAYBACK_STATE playbackState;
            _instance.getPlaybackState(out playbackState);
            return (playbackState != FMOD.Studio.PLAYBACK_STATE.STOPPED);
        }
        return false;
    }

    protected override void HandleGameEvent(EmitterGameEvent gameEvent) {
        if(_playEvent == gameEvent) {
            Play();
        }
        if(_stopEvent == gameEvent) {
            Stop();
        }
    }
}
