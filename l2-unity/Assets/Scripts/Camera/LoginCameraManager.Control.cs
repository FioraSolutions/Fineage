using System;

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
