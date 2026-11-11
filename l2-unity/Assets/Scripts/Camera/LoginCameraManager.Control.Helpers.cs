using System;

public partial class LoginCameraManager : MonoBehaviour
{
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
