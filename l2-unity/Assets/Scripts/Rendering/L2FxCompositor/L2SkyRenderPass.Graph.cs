using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public sealed partial class L2SkyRenderPass
{
    public L2SkyRenderPass(L2FxCompositorSettings settings)
    {
        _settings = settings;
        // Deferred opaque depth/color is ready here. Sky shaders force far-plane
        // depth, so every world surface remains in front of the backdrop.
        renderPassEvent = RenderPassEvent.AfterRenderingDeferredLights;
    }

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

    public void Dispose()
    {
        CoreUtils.Destroy(_backdropMaterial);
        _backdropMaterial = null;
    }
}
