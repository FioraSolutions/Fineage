using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class AmbientSoundEmitter : EventHandler
{
    private bool ShouldStop()
    {
        var listener = AudioPacketServices.Listener;
        if (listener == null) return true;

        var player = listener.Player;
        if (player == null) return true;

        float distance = Vector3.Distance(transform.position, player.transform.position);
        return distance > MaxDistance;
    }

    private void PlayInstance() {
        if (!_instance.isValid()) {
            _instance.clearHandle();
        }

        // Let previous oneshot instances play out
        if (_instance.isValid()) {
            _instance.release();
            _instance.clearHandle();
        }

        bool is3D;
        _eventDescription.is3D(out is3D);

        if(!_instance.isValid()) {
            _eventDescription.createInstance(out _instance);

            // Only want to update if we need to set 3D attributes
            if(is3D) {
                _instance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
            }
        }

        if(is3D && _overrideAttenuation) {
            _instance.setProperty(FMOD.Studio.EVENT_PROPERTY.MINIMUM_DISTANCE, _overrideMinDistance);
            _instance.setProperty(FMOD.Studio.EVENT_PROPERTY.MAXIMUM_DISTANCE, _overrideMaxDistance);
        }

        _instance.setPitch(_soundPitch);
        _instance.start();
    }

    public void Stop() {
        StopCoroutine(StartPlayLoop());
        StopInstance();
    }
}
