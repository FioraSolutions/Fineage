using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class LoginCameraManager : MonoBehaviour
{
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

        // Race/gender cams are not the credentials Login camera — hide forest root.
        if (camera == null || camera.name != "Login")
            LoginForestPresenter.Dismiss();
        else
            LoginForestPresenter.Present(camera);

        EnsureStudioListener(camera);
        UpdateListenerPosition();
    }
}
