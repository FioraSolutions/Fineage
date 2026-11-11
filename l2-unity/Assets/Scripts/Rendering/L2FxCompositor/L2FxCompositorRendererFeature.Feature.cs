using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public sealed partial class L2FxCompositorRendererFeature : ScriptableRendererFeature
{
    bool WantYebis(ref RenderingData renderingData)
    {
        if (settings == null || !settings.enableYebisPost || !isActive || _yebisMaterial == null)
            return false;

        if (settings.gameCameraOnly)
        {
            CameraType type = renderingData.cameraData.camera.cameraType;
            if (type != CameraType.Game && type != CameraType.VR)
                return false;
        }

        Camera renderingCamera = renderingData.cameraData.camera;
        CameraController controller = CameraPacketServices.Controller;
        if (controller == null || !controller.isActiveAndEnabled)
            return false;

        Camera worldCamera = controller.GetComponent<Camera>();
        if (worldCamera == null)
            worldCamera = controller.GetComponentInChildren<Camera>();

        return worldCamera != null &&
               worldCamera.enabled &&
               worldCamera.gameObject.activeInHierarchy &&
               renderingCamera == worldCamera;
    }

    protected override void Dispose(bool disposing)
    {
        _skyPass?.Dispose();
        _pass?.Dispose();
        _yebisPass?.Dispose();
        _pass = null;
        _skyPass = null;
        _yebisPass = null;
        _terrainOverlayPass = null;
        CoreUtils.Destroy(_transferMaterial);
        _transferMaterial = null;
        CoreUtils.Destroy(_postMaterial);
        _postMaterial = null;
        CoreUtils.Destroy(_yebisMaterial);
        _yebisMaterial = null;
        L2FxGpuDrawQueue.Clear();
        L2NameplateOverlayQueue.Clear();
        base.Dispose(disposing);
    }
}
