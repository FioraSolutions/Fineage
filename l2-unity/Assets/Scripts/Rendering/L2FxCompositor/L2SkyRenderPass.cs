using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Reconstructs the original L2 colour-pass sky.
/// Upper sky / sun / clouds stay behind world depth. Hard lower haze
/// matches EID 1475 and fills the whole below-horizon buffer.
/// </summary>
public sealed class L2SkyRenderPass : ScriptableRenderPass
{
    static readonly int D3D9ActiveId = Shader.PropertyToID("_L2FxD3D9CompositorActive");
    static readonly int FxGainId = Shader.PropertyToID("_FxGain");
    static readonly int SkyColorId = Shader.PropertyToID("_SkyColor");
    static readonly int LowerHazeColorId = Shader.PropertyToID("_LowerHazeColor");
    static readonly ShaderTagId ForwardOnlyTag = new ShaderTagId("UniversalForwardOnly");
    static readonly ProfilingSampler Sampler = new ProfilingSampler("L2 Original Sky Layers");

    readonly L2FxCompositorSettings _settings;
    Material _backdropMaterial;

    sealed class PassData
    {
        public TextureHandle color;
        public TextureHandle depth;
        public bool hasDepth;
        public Material backdrop;
        public Color skyColor;
        public Color hazeColor;
        public RendererListHandle celestialHigh;
        public RendererListHandle celestialLow;
        public RendererListHandle clouds;
        public RendererListHandle haze;
        public bool hasCelestialHigh;
        public bool hasCelestialLow;
        public bool hasClouds;
        public bool hasHaze;
    }

    public L2SkyRenderPass(L2FxCompositorSettings settings)
    {
        _settings = settings;
        // Deferred opaque depth/color is ready here. Sky shaders force far-plane
        // depth, so every world surface remains in front of the backdrop.
        renderPassEvent = RenderPassEvent.AfterRenderingDeferredLights;
    }

#if URP_COMPATIBILITY_MODE
#pragma warning disable 618, 672
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
#pragma warning restore 618, 672
#endif

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
        if (!ShouldRun(cameraData.camera))
            return;

        EnsureBackdropMaterial();

        UniversalResourceData resources = frameData.Get<UniversalResourceData>();
        UniversalRenderingData renderingData = frameData.Get<UniversalRenderingData>();
        UniversalLightData lightData = frameData.Get<UniversalLightData>();

        TextureHandle color = resources.activeColorTexture;
        if (!color.IsValid())
            return;

        TextureHandle depth = resources.activeDepthTexture;
        bool hasDepth = depth.IsValid();

        using (IUnsafeRenderGraphBuilder builder =
               renderGraph.AddUnsafePass<PassData>("L2 Original Sky Layers", out PassData passData, Sampler))
        {
            passData.color = color;
            passData.depth = depth;
            passData.hasDepth = hasDepth;
            passData.backdrop = _backdropMaterial;
            passData.skyColor = ResolveSkyColor(cameraData.camera);
            passData.hazeColor = L2HazeRing.CurrentTint;

            passData.hasCelestialHigh = TryCreateLayerList(
                renderGraph, renderingData, cameraData, lightData,
                _settings.celestialLayerMask, QueueRange(3000, 5000),
                out passData.celestialHigh);
            passData.hasCelestialLow = TryCreateLayerList(
                renderGraph, renderingData, cameraData, lightData,
                _settings.celestialLayerMask, QueueRange(2501, 2999),
                out passData.celestialLow);
            passData.hasClouds = TryCreateLayerList(
                renderGraph, renderingData, cameraData, lightData,
                _settings.cloudLayerMask, RenderQueueRange.transparent,
                out passData.clouds);
            passData.hasHaze = TryCreateLayerList(
                renderGraph, renderingData, cameraData, lightData,
                _settings.hazeLayerMask, RenderQueueRange.transparent,
                out passData.haze);

            builder.UseTexture(color, AccessFlags.ReadWrite);
            if (hasDepth)
                builder.UseTexture(depth, AccessFlags.ReadWrite);

            if (passData.hasCelestialHigh)
                builder.UseRendererList(passData.celestialHigh);
            if (passData.hasCelestialLow)
                builder.UseRendererList(passData.celestialLow);
            if (passData.hasClouds)
                builder.UseRendererList(passData.clouds);
            if (passData.hasHaze)
                builder.UseRendererList(passData.haze);

            builder.AllowPassCulling(false);
            builder.AllowGlobalStateModification(true);
            builder.SetRenderFunc(static (PassData data, UnsafeGraphContext context) =>
            {
                CommandBuffer cmd = CommandBufferHelpers.GetNativeCommandBuffer(context.cmd);

                if (data.hasDepth)
                    cmd.SetRenderTarget(data.color, data.depth);
                else
                    cmd.SetRenderTarget(data.color);

                cmd.SetGlobalFloat(D3D9ActiveId, 1f);
                cmd.SetGlobalFloat(FxGainId, 1f);

                if (data.backdrop != null)
                {
                    data.backdrop.SetVector(SkyColorId, data.skyColor);
                    data.backdrop.SetVector(LowerHazeColorId, data.hazeColor);
                    cmd.DrawProcedural(
                        Matrix4x4.identity,
                        data.backdrop,
                        0,
                        MeshTopology.Triangles,
                        3,
                        1);
                }

                if (data.hasCelestialHigh)
                    cmd.DrawRendererList(data.celestialHigh);

                if (data.backdrop != null)
                {
                    cmd.SetRenderTarget(data.color);
                    cmd.DrawProcedural(
                        Matrix4x4.identity,
                        data.backdrop,
                        1,
                        MeshTopology.Triangles,
                        3,
                        1);
                    if (data.hasDepth)
                        cmd.SetRenderTarget(data.color, data.depth);
                }

                if (data.hasCelestialLow)
                    cmd.DrawRendererList(data.celestialLow);
                if (data.hasClouds)
                    cmd.DrawRendererList(data.clouds);
                if (data.hasHaze)
                    cmd.DrawRendererList(data.haze);

                cmd.SetGlobalFloat(D3D9ActiveId, 0f);
            });
        }
    }

    bool TryCreateLayerList(
        RenderGraph renderGraph,
        UniversalRenderingData renderingData,
        UniversalCameraData cameraData,
        UniversalLightData lightData,
        LayerMask layerMask,
        RenderQueueRange queueRange,
        out RendererListHandle handle)
    {
        handle = default;
        if (layerMask.value == 0)
            return false;

        var filtering = new FilteringSettings(queueRange, layerMask);
        DrawingSettings drawing = RenderingUtils.CreateDrawingSettings(
            ForwardOnlyTag,
            renderingData,
            cameraData,
            lightData,
            SortingCriteria.CommonTransparent);
        drawing.perObjectData = PerObjectData.None;

        var param = new RendererListParams(renderingData.cullResults, drawing, filtering);
        handle = renderGraph.CreateRendererList(param);
        return handle.IsValid();
    }

    static RenderQueueRange QueueRange(int lower, int upper)
    {
        return new RenderQueueRange
        {
            lowerBound = lower,
            upperBound = upper
        };
    }

    static Color ResolveSkyColor(Camera camera)
    {
        Material skybox = RenderSettings.skybox;
        if (skybox != null && skybox.HasProperty("_GradientColor1"))
            return skybox.GetColor("_GradientColor1");

        return camera != null ? camera.backgroundColor : Color.black;
    }

    public void Dispose()
    {
        CoreUtils.Destroy(_backdropMaterial);
        _backdropMaterial = null;
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

        CameraController controller = CameraController.Instance;
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
