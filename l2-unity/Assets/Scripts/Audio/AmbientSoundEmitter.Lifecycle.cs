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

    public void Play() {
        if(_eventReference.IsNull) {
            return;
        }

        if(!_eventDescription.isValid()) {
            Lookup();
        }

        if (_loop) {
            StopCoroutine(StartPlayLoop());
            StartCoroutine(StartPlayLoop());
            return;
        }

        if (_isSound3D && ShouldStop()) {
            Stop();
        } else if (ShouldPlayEvent()) {
            PlayInstance();
        }
    }

    IEnumerator StartPlayLoop() {
        while(true) {
            if (_isSound3D && ShouldStop()) {
                Stop();
                yield break;
            }
            if(ShouldPlayEvent()) {
                PlayInstance();
            }
            yield return new WaitForSeconds(_clipLengthSeconds * 0.99f); // 1% for error margin
            if (_loopDelaySeconds > 0) {
                yield return new WaitForSeconds(_loopDelaySeconds);
            }
        }
    }

    private bool ShouldPlayEvent() {
        WorldClock clock = WorldClockPacketServices.Clock;
        if (clock != null)
        {
            if (_ambientSoundType == AmbientSoundType.AST1_Day && clock.IsNightTime())
            {
                return false;
            }

            if (_ambientSoundType == AmbientSoundType.AST1_Night && !clock.IsNightTime())
            {
                return false;
            }
        }

        if(Random.Range(1, 100) <= _playChancePercent) {
            return true;
        }

        return false;
    }
}
