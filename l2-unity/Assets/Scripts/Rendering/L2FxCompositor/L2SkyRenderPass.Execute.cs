using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public sealed partial class L2SkyRenderPass
{
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (!ShouldRun(renderingData.cameraData.camera))
            return;

        EnsureBackdropMaterial();
        RTHandle cameraColor = renderingData.cameraData.renderer.cameraColorTargetHandle;
        RTHandle cameraDepth = renderingData.cameraData.renderer.cameraDepthTargetHandle;
        if (cameraColor == null)
            return;

        CommandBuffer cmd = CommandBufferPool.Get("L2 Original Sky Layers");
        using (new ProfilingScope(cmd, Sampler))
        {
            if (cameraDepth != null)
                CoreUtils.SetRenderTarget(cmd, cameraColor, cameraDepth, ClearFlag.None, Color.clear);
            else
                CoreUtils.SetRenderTarget(cmd, cameraColor, ClearFlag.None, Color.clear);
            cmd.SetGlobalFloat(D3D9ActiveId, 1f);
            cmd.SetGlobalFloat(FxGainId, 1f);

            if (_backdropMaterial != null)
            {
                _backdropMaterial.SetVector(SkyColorId, ResolveSkyColor(renderingData.cameraData.camera));
                _backdropMaterial.SetVector(LowerHazeColorId, L2HazeRing.CurrentTint);
                cmd.DrawProcedural(
                    Matrix4x4.identity,
                    _backdropMaterial,
                    0,
                    MeshTopology.Triangles,
                    3,
                    1);
            }
        }

        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();

        DrawLayer(
            context,
            ref renderingData,
            _settings.celestialLayerMask,
            QueueRange(3000, 5000));

        if (_backdropMaterial != null)
        {
            CoreUtils.SetRenderTarget(cmd, cameraColor, ClearFlag.None, Color.clear);
            cmd.DrawProcedural(
                Matrix4x4.identity,
                _backdropMaterial,
                1,
                MeshTopology.Triangles,
                3,
                1);
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            if (cameraDepth != null)
                CoreUtils.SetRenderTarget(cmd, cameraColor, cameraDepth, ClearFlag.None, Color.clear);
        }

        DrawLayer(
            context,
            ref renderingData,
            _settings.celestialLayerMask,
            QueueRange(2501, 2999));
        DrawLayer(context, ref renderingData, _settings.cloudLayerMask);
        DrawLayer(context, ref renderingData, _settings.hazeLayerMask);

        cmd.SetGlobalFloat(D3D9ActiveId, 0f);
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    void DrawLayer(
        ScriptableRenderContext context,
        ref RenderingData renderingData,
        LayerMask layerMask,
        RenderQueueRange? queueRange = null)
    {
        if (layerMask.value == 0)
            return;

        var filtering = new FilteringSettings(
            queueRange ?? RenderQueueRange.transparent,
            layerMask);
        DrawingSettings drawing = CreateDrawingSettings(
            ForwardOnlyTag,
            ref renderingData,
            SortingCriteria.CommonTransparent);
        drawing.perObjectData = PerObjectData.None;
        context.DrawRenderers(renderingData.cullResults, ref drawing, ref filtering);
    }

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
