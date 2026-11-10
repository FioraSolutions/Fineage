using System;

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginCameraManager : MonoBehaviour
{
    private Dictionary<string, Camera> cameras = new Dictionary<string, Camera>();

    private Camera _activeCamera;

    private static LoginCameraManager _instance;
    [Obsolete("Use [Inject] / PacketServices instead")]
    public static LoginCameraManager Instance { get { return _instance; } }

    void Awake() {
        if (_instance == null) {
            _instance = this;
        } else if (_instance != this) {
            Destroy(this);
            return;
        }

        // Always-on listener on this manager (cameras get enabled/disabled; FMOD counts OnEnable).
        if (GetComponent<FMODUnity.StudioListener>() == null)
            gameObject.AddComponent<FMODUnity.StudioListener>();

        cameras.Add("Login", GameObject.Find("Login").GetComponent<Camera>());
        cameras.Add("CharSelect", GameObject.Find("CharSelect").GetComponent<Camera>());
        cameras.Add("Dark Elf", GameObject.Find("DarkElf").GetComponent<Camera>());
        cameras.Add("Orc", GameObject.Find("Orc").GetComponent<Camera>());
        cameras.Add("Dwarf", GameObject.Find("Dwarf").GetComponent<Camera>());
        cameras.Add("Elf", GameObject.Find("Elf").GetComponent<Camera>());
        cameras.Add("Human", GameObject.Find("Human").GetComponent<Camera>());

        DisableCameras();
        SwitchCamera("Login");
    }

    private void Start() {
        // Cameras wired in Awake so StudioListener exists before FMOD's first Update.
        EnsureStudioListener(_activeCamera);
        UpdateListenerPosition();
    }

    public Camera SelectClassCamera(string race, string charClass) {
        if(cameras.TryGetValue(race, out Camera camera)) {
            if(charClass == "Fighter") {
                return camera.transform.GetChild(0).GetComponent<Camera>();
            }

            return camera.transform.GetChild(1).GetComponent<Camera>();
        }


        return null;
    }

    public Camera SelectGenderCamera(string race, string charClass, string gender) {
        Camera classCamera = SelectClassCamera(race, charClass);
        if (classCamera != null) {
            if (gender == "Male") {
                return classCamera.transform.GetChild(0).GetComponent<Camera>();
            }

            return classCamera.transform.GetChild(1).GetComponent<Camera>();
        }

        return null;
    }

    public Camera SelectHeadCamera(string race, string charClass, string gender) {
        Camera genderCamera = SelectGenderCamera(race, charClass, gender);
        if (genderCamera != null) {
            return genderCamera.transform.GetChild(0).GetComponent<Camera>();
        }

        return null;
    }

    public void SwitchCamera(Camera camera) {
        DisableLoginCameras();

        LobbyPresentation.EnsureCameraSeesLobby(camera);
        camera.enabled = true;
        _activeCamera = camera;

        EnsureStudioListener(camera);
        UpdateListenerPosition();
    }

    public void SwitchCamera(string camera)
     {
        DisableLoginCameras();

        if (cameras.TryGetValue(camera, out Camera obj))
         {
            // Debug.Log(camera + " camera enabled.");
            LobbyPresentation.EnsureCameraSeesLobby(obj);
            obj.enabled = true;
            _activeCamera = obj;

            if (camera == "CharSelect")
            {
                LobbyNameplatesManager lobbyPlates = NameplatePacketServices.LobbyNameplates;
                if (lobbyPlates != null)
                {
                    lobbyPlates.Camera = obj;
                }

                CharacterSelector selector = LoginPacketServices.Characters;
                if (selector != null)
                {
                    selector.Camera = obj;
                }
            }
            else
            {
                LobbyNameplatesManager lobbyPlates = NameplatePacketServices.LobbyNameplates;
                if (lobbyPlates != null)
                {
                    lobbyPlates.Camera = null;
                }

                CharacterSelector selector = LoginPacketServices.Characters;
                if (selector != null)
                {
                    selector.Camera = null;
                }
            }

            EnsureStudioListener(obj);
            UpdateListenerPosition();
        }
    }

    public void DisableCameras() {
        foreach (var cam in cameras.Values) {
            cam.enabled = false;
        }
    }

    /// <summary>
    /// Only login/char-select cameras. Never touch gameplay <see cref="CameraController"/> —
    /// <c>Camera.main</c> can be the world camera while Game is preloaded.
    /// </summary>
    public void DisableLoginCameras() {
        if (_activeCamera != null)
            _activeCamera.enabled = false;
        DisableCameras();
    }

    private void UpdateListenerPosition() {
        if (_activeCamera == null)
            return;

        EnsureStudioListener(_activeCamera);

        var listener = AudioPacketServices.Listener;
        if (listener == null)
            return;

        listener.transform.position = _activeCamera.transform.position;
        listener.Cam = _activeCamera.gameObject;
    }

    /// <summary>
    /// Menu scene often has no ThirdPersonListener; FMOD still needs a StudioListener
    /// or RuntimeManager spams the console on login.
    /// </summary>
    static void EnsureStudioListener(Camera cam)
    {
        if (cam == null)
            return;
        if (cam.GetComponent<FMODUnity.StudioListener>() == null)
            cam.gameObject.AddComponent<FMODUnity.StudioListener>();
    }

    public void ZoomIn() {
        SwitchCamera(_activeCamera.transform.GetChild(0).GetComponent<Camera>());
    }

    public void ZoomOut() {
        SwitchCamera(_activeCamera.transform.parent.GetComponent<Camera>());
    }
}
