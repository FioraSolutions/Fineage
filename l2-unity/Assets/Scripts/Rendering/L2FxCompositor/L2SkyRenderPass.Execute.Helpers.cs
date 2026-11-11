using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public sealed partial class L2SkyRenderPass
{
    static Color ResolveSkyColor(Camera camera)
    {
        Material skybox = RenderSettings.skybox;
        if (skybox != null && skybox.HasProperty("_GradientColor1"))
            return skybox.GetColor("_GradientColor1");

        return camera != null ? camera.backgroundColor : Color.black;
    }

    void EnsureBackdropMaterial()
    {
        if (_backdropMaterial != null)
            return;

        Shader backdropShader = Shader.Find("Hidden/L2/SkyBackdrop");
        if (backdropShader != null)
            _backdropMaterial = CoreUtils.CreateEngineMaterial(backdropShader);
    }

    bool ShouldRun(Camera renderingCamera)
    {
        if (_settings == null ||
            !_settings.enableD3D9Compositor ||
            !_settings.directCameraColor)
        {
            return false;
        }

        CameraType type = renderingCamera.cameraType;
        if (_settings.gameCameraOnly &&
            type != CameraType.Game &&
            type != CameraType.VR)
        {
            return false;
        }

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
}
