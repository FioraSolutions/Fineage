using FMODUnity;
using System.Collections;
using UnityEngine;
using FMOD.Studio;

public partial class MusicAreaTrigger : MonoBehaviour
{
    private void Awake() {
        if (!_eventDescription.isValid()) {
            Lookup();
        }

        int lengthMs = 0;
        _eventDescription.getLength(out lengthMs);
        _musicLengthSeconds = lengthMs / 1000f;
    }

    private void Lookup() {
        _eventDescription = RuntimeManager.GetEventDescription(_enterEvent);
    }

    private void OnTriggerEnter(Collider other) {
        // Debug.Log("Enter area:" + transform.name);
        PlayMusic();
    }
}
