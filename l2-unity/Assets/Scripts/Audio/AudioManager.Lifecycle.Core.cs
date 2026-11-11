using System;
using FMODUnity;
using UnityEngine;
using FMOD.Studio;

public partial class AudioManager : MonoBehaviour
{
    private void Awake()
    {
        if (_instance == null)
        {
            if (_isDisabled)
            {
                // Debug.Log("AUDIO MANAGER: WARNING SOUND DISABLED");
            }

            _instance = this;
            AudioPacketServices.Bind(audio: this);
        }
        else
        {
            Destroy(this);
        }

        SetBuses();
    }

    private void SetBuses()
    {
        _masterBus = RuntimeManager.GetBus("bus:/");
        _musicBus = RuntimeManager.GetBus("bus:/Music");
        _SFXBus = RuntimeManager.GetBus("bus:/SFX");
        _UIBus = RuntimeManager.GetBus("bus:/UI");
        _ambientBus = RuntimeManager.GetBus("bus:/Ambient");
    }

    private void Update()
    {
        if (GameSettings.Mute || (_muteWhenNotFocused && !Application.isFocused))
        {
            _masterBus.setVolume(0);
            _musicBus.setVolume(0);
            _SFXBus.setVolume(0);
            _UIBus.setVolume(0);
            _ambientBus.setVolume(0);
        }
        else
        {
            _masterBus.setVolume(_masterVolume * 0.75f * GameSettings.Master);
            _musicBus.setVolume(_musicVolume * 0.7f * GameSettings.Music);
            _SFXBus.setVolume(_SFXVolume * GameSettings.Sfx);
            _UIBus.setVolume(_UIVolume * GameSettings.Sfx);
            _ambientBus.setVolume(_ambientVolume * 0.45f * GameSettings.Ambient);
        }
    }
}
