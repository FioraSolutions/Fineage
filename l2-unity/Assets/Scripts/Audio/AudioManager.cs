using System;

public partial class AudioManager : MonoBehaviour
{
    [Header("Volume")]
    [Range(0, 1)]
    [SerializeField] private float _masterVolume = 1;
    [Range(0, 1)]
    [SerializeField] private float _musicVolume = 1;
    [Range(0, 1)]
    [SerializeField] private float _SFXVolume = 1;
    [Range(0, 1)]
    [SerializeField] private float _UIVolume = 1;
    [Range(0, 1)]
    [SerializeField] private float _ambientVolume = 1;
    [SerializeField] private bool _muteWhenNotFocused = true;

    private Bus _masterBus;
    private Bus _musicBus;
    private Bus _SFXBus;
    private Bus _UIBus;
    private Bus _ambientBus;
    private bool _isDisabled = true;

    private static AudioManager _instance;

    [System.Obsolete("Use AudioPacketServices.Audio or [Inject] AudioManager instead")]
    public static AudioManager Instance { get { return _instance; } }

}
