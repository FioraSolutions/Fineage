using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class LoginCameraManager : MonoBehaviour
{
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

    public void DisableLoginCameras() {
        if (_activeCamera != null)
            _activeCamera.enabled = false;
        DisableCameras();
    }
}
