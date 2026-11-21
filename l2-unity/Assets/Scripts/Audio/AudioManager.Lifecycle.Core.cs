using System;
using FMODUnity;
using UnityEngine;
using FMOD.Studio;

public partial class AudioManager : MonoBehaviour
{
    AudioService _service;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            EnsureService();
            AudioPacketServices.Bind(audio: this, service: _service);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
            return;
        }
    }

    internal void BindService(AudioService service) => _service = service;

    void EnsureService()
    {
        if (_service == null)
            _service = GetComponent<AudioService>();
        if (_service == null)
            _service = gameObject.AddComponent<AudioService>();
        _service.SyncInspectorVolumes(
            _masterVolume,
            _musicVolume,
            _ambientVolume,
            _SFXVolume,
            _UIVolume,
            voiceVolume: 1f,
            _muteWhenNotFocused,
            _isDisabled);
    }

    AudioService Svc
    {
        get
        {
            if (_service == null)
                EnsureService();
            return _service;
        }
    }
}