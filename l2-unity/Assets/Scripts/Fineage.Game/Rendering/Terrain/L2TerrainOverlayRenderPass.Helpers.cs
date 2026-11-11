using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public sealed partial class L2TerrainOverlayRenderPass : ScriptableRenderPass
{
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CameraType type = renderingData.cameraData.camera.cameraType;
        if (type != CameraType.Game && type != CameraType.SceneView && type != CameraType.VR)
            return;

        L2TerrainLayerStack.EnsureAttached();

        RTHandle cameraColor = renderingData.cameraData.renderer.cameraColorTargetHandle;
        RTHandle cameraDepth = renderingData.cameraData.renderer.cameraDepthTargetHandle;
        if (cameraColor == null)
            return;

        CommandBuffer cmd = CommandBufferPool.Get("L2 Terrain Overlays");
        using (new ProfilingScope(cmd, Sampler))
        {
            if (cameraDepth != null)
                CoreUtils.SetRenderTarget(cmd, cameraColor, cameraDepth, ClearFlag.None, Color.clear);
            else
                CoreUtils.SetRenderTarget(cmd, cameraColor, ClearFlag.None, Color.clear);
            L2TerrainLayerStack.DrawAll(cmd);
        }

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
        CameraType type = cameraData.camera.cameraType;
        if (type != CameraType.Game && type != CameraType.SceneView && type != CameraType.VR)
            return;

        L2TerrainLayerStack.EnsureAttached();

        UniversalResourceData resources = frameData.Get<UniversalResourceData>();
        TextureHandle color = resources.activeColorTexture;
        if (!color.IsValid())
            return;

        TextureHandle depth = resources.activeDepthTexture;
        bool hasDepth = depth.IsValid();

        using (IUnsafeRenderGraphBuilder builder =
               renderGraph.AddUnsafePass<PassData>("L2 Terrain Overlays", out PassData passData, Sampler))
        {
            passData.color = color;
            passData.depth = depth;
            passData.hasDepth = hasDepth;

            builder.UseTexture(color, AccessFlags.ReadWrite);
            if (hasDepth)
                builder.UseTexture(depth, AccessFlags.ReadWrite);

            builder.AllowPassCulling(false);
            builder.AllowGlobalStateModification(true);
            builder.SetRenderFunc(static (PassData data, UnsafeGraphContext context) =>
            {
                CommandBuffer cmd = CommandBufferHelpers.GetNativeCommandBuffer(context.cmd);
                if (data.hasDepth)
                    cmd.SetRenderTarget(data.color, data.depth);
                else
                    cmd.SetRenderTarget(data.color);
                L2TerrainLayerStack.DrawAll(cmd);
            });
        }
    }
}
