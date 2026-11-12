using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class LoginCameraManager : MonoBehaviour
{
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
        // Forest login vista: SwitchCamera("Login") → LoginForestPresenter.Present.
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
}
