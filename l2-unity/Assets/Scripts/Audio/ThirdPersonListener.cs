using System;

using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class ThirdPersonListener : MonoBehaviour
{
    [SerializeField] private GameObject _player, _cam;
    [SerializeField] private int _listener;

    public GameObject Player { get { return _player; } }
    public GameObject Cam { get { return _cam; } set { _cam = value; } }

    private FMOD.ATTRIBUTES_3D _attributes = new FMOD.ATTRIBUTES_3D();

    private static ThirdPersonListener _instance;

    [System.Obsolete("Use AudioPacketServices.Listener or [Inject] ThirdPersonListener instead")]
    public static ThirdPersonListener Instance { get { return _instance; } }

    private PlayerController ResolvedPlayer => AudioPacketServices.Player;

    private void Awake() {
        if (_instance == null) {
            _instance = this;
            AudioPacketServices.Bind(listener: this);
        }

        if (_cam == null && Camera.main != null) {
            _cam = Camera.main.gameObject;
        }

        App.InjectGameObject(gameObject);
        WireAudioPacketServices();
    }

    private void OnEnable() {
        _instance = this;
        AudioPacketServices.Bind(listener: this);
    }

    private void Start()
    {
        WireAudioPacketServices();
    }

    private void WireAudioPacketServices()
    {
        AudioPacketServices.Bind(player: ResolvedPlayer, listener: this);
    }

    private void OnDisable() {
        if (_instance == this) {
            _instance = FindActiveListener();
        }
    }

    void OnDestroy() {
        if (_instance == this) {
            _instance = null;
        }
    }

    private ThirdPersonListener FindActiveListener() {
        ThirdPersonListener[] listeners = FindObjectsByType<ThirdPersonListener>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        for (int i = 0; i < listeners.Length; i++) {
            if (listeners[i] != null && listeners[i] != this && listeners[i].isActiveAndEnabled) {
                return listeners[i];
            }
        }

        return null;
    }

    void Update() {
        if (_player == null) {
            PlayerController playerController = ResolvedPlayer;
            if (playerController != null)
                _player = playerController.gameObject;
        }

        if (_cam == null && Camera.main != null)
            _cam = Camera.main.gameObject;

        if (_cam == null)
            return;

        // Interlude-style 3D pan: listener at camera position/orientation (not player feet).
        _attributes.position = FMODUnity.RuntimeUtils.ToFMODVector(_cam.transform.position);

        FMODUnity.StudioListener studio = _cam.GetComponent<FMODUnity.StudioListener>();
        if (studio == null)
            studio = _cam.AddComponent<FMODUnity.StudioListener>();

        _attributes.forward = FMODUnity.RuntimeUtils.ToFMODVector(_cam.transform.forward);
        _attributes.up = FMODUnity.RuntimeUtils.ToFMODVector(_cam.transform.up);
        FMODUnity.RuntimeManager.StudioSystem.setListenerAttributes(_listener, _attributes);
    }
}

