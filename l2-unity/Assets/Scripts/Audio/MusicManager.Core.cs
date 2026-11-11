using System;
using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public partial class MusicManager : MonoBehaviour
{
    private void Awake() {
        if (_instance == null) {
            _instance = this;
            MusicPacketServices.Bind(music: this);
        } else if(_instance != this){
            Destroy(this);
            return;
        }

        _musicInstances = new Dictionary<EventReference, EventInstance>();
    }

    void OnDestroy() {
        if (_musicInstances == null)
            return; // duplicate destroyed in Awake
        Clear();
    }

    public void Clear() {
        ResetPriority();
        StopMusic(CurrentMusicEvent);
        _musicInstances.Clear();
    }
}
