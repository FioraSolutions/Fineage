using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public sealed partial class L2FxCompositorRenderPass
{
    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        if (_settings != null && _settings.directCameraColor)
            return;

        RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;
        desc.msaaSamples = 1;
        desc.depthBufferBits = 0;
        desc.graphicsFormat = GraphicsFormat.R8G8B8A8_UNorm;
        desc.sRGB = false;

        RenderingUtils.ReAllocateIfNeeded(
            ref _l2Color,
            desc,
            FilterMode.Bilinear,
            TextureWrapMode.Clamp,
            name: "_L2FxD3D9Color");

        if (!WantSkillPost())
            return;

        RenderingUtils.ReAllocateIfNeeded(
            ref _l2Scene,
            desc,
            FilterMode.Bilinear,
            TextureWrapMode.Clamp,
            name: "_L2FxScene");

        RenderTextureDescriptor halfDesc = desc;
        halfDesc.width = Mathf.Max(1, desc.width / 2);
        halfDesc.height = Mathf.Max(1, desc.height / 2);

        RenderingUtils.ReAllocateIfNeeded(
            ref _l2BloomA,
            halfDesc,
            FilterMode.Bilinear,
            TextureWrapMode.Clamp,
            name: "_L2FxBloomA");
        RenderingUtils.ReAllocateIfNeeded(
            ref _l2BloomB,
            halfDesc,
            FilterMode.Bilinear,
            TextureWrapMode.Clamp,
            name: "_L2FxBloomB");
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (_settings != null && _settings.directCameraColor)
        {
            ExecuteDirect(ref context, ref renderingData);
            return;
        }

        if (_transferMaterial == null || _l2Color == null)
        {
            L2FxGpuDrawQueue.FlushImmediateFallback();
            if (!L2YebisPostRuntime.WillFlushNameplates)
                L2NameplateOverlayQueue.FlushImmediateFallback();
            return;
        }

        if (!ShouldRun(ref renderingData))
        {
            L2FxGpuDrawQueue.FlushImmediateFallback();
            if (!L2YebisPostRuntime.WillFlushNameplates)
                L2NameplateOverlayQueue.FlushImmediateFallback();
            Shader.SetGlobalFloat(D3D9ActiveId, 0f);
            return;
        }

        // CPU-only scope: DrawSkillFx/DrawFxLayer call cmd.Clear() after
        // ExecuteCommandBuffer, which would wipe BeginSample and spam
        // Begin/EndProfileEvent mismatch errors if the scope targeted cmd.
        CommandBuffer cmd = CommandBufferPool.Get("L2Fx D3D9 Compositor");
        using (new ProfilingScope(Sampler))
        {
            Shader.SetGlobalFloat(D3D9ActiveId, 1f);
            Shader.SetGlobalFloat(DebugModeId, _settings.debugMode);

            RTHandle cameraColor = renderingData.cameraData.renderer.cameraColorTargetHandle;
            RTHandle cameraDepth = renderingData.cameraData.renderer.cameraDepthTargetHandle;

            if (UseSkillPost())
                ExecuteSkillPost(context, ref renderingData, cmd, cameraColor, cameraDepth);
            else
                ExecuteLegacyComposite(context, ref renderingData, cmd, cameraColor, cameraDepth);

            if (!L2YebisPostRuntime.WillFlushNameplates)
                DrawNameplatesAfterComposite(cmd, cameraColor);
        }

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
        if (!ShouldRun(cameraData.camera))
        {
            L2FxGpuDrawQueue.FlushImmediateFallback();
            if (!L2YebisPostRuntime.WillFlushNameplates)
                L2NameplateOverlayQueue.FlushImmediateFallback();
            Shader.SetGlobalFloat(D3D9ActiveId, 0f);
            return;
        }

        // Default project path: direct UNORM into camera color.
        // Legacy encode/post path still needs Compatibility Mode or a fuller RG port.
        if (_settings == null || !_settings.directCameraColor)
        {
            L2FxGpuDrawQueue.FlushImmediateFallback();
            if (!L2YebisPostRuntime.WillFlushNameplates)
                L2NameplateOverlayQueue.FlushImmediateFallback();
            Shader.SetGlobalFloat(D3D9ActiveId, 0f);
            return;
        }

        UniversalResourceData resources = frameData.Get<UniversalResourceData>();
        UniversalRenderingData renderingData = frameData.Get<UniversalRenderingData>();
        UniversalLightData lightData = frameData.Get<UniversalLightData>();

        TextureHandle color = resources.activeColorTexture;
        if (!color.IsValid())
        {
            L2FxGpuDrawQueue.FlushImmediateFallback();
            if (!L2YebisPostRuntime.WillFlushNameplates)
                L2NameplateOverlayQueue.FlushImmediateFallback();
            return;
        }

        TextureHandle depth = resources.activeDepthTexture;
        bool hasDepth = depth.IsValid();
        bool flushNameplates = !L2YebisPostRuntime.WillFlushNameplates;

        using (IUnsafeRenderGraphBuilder builder =
               renderGraph.AddUnsafePass<DirectPassData>("L2 FX Direct UNORM", out DirectPassData passData, SamplerDirect))
        {
            passData.color = color;
            passData.depth = depth;
            passData.hasDepth = hasDepth;
            passData.bindDepth = _settings.bindCameraDepth;
            passData.debugMode = _settings.debugMode;
            passData.flushNameplates = flushNameplates;

            DrawingSettings drawing = RenderingUtils.CreateDrawingSettings(
                ForwardOnlyTag,
                renderingData,
                cameraData,
                lightData,
                SortingCriteria.SortingLayer | SortingCriteria.RenderQueue);
            drawing.perObjectData = PerObjectData.None;
            var param = new RendererListParams(renderingData.cullResults, drawing, _filtering);
            passData.fxList = renderGraph.CreateRendererList(param);
            passData.hasFxList = passData.fxList.IsValid();

            builder.UseTexture(color, AccessFlags.ReadWrite);
            if (hasDepth)
                builder.UseTexture(depth, AccessFlags.ReadWrite);
            if (passData.hasFxList)
                builder.UseRendererList(passData.fxList);

            builder.AllowPassCulling(false);
            builder.AllowGlobalStateModification(true);
            builder.SetRenderFunc(static (DirectPassData data, UnsafeGraphContext context) =>
            {
                CommandBuffer cmd = CommandBufferHelpers.GetNativeCommandBuffer(context.cmd);
                cmd.SetGlobalFloat(D3D9ActiveId, 1f);
                cmd.SetGlobalFloat(DebugModeId, data.debugMode);
                cmd.SetGlobalFloat(FxGainId, 1f);

                if (data.bindDepth && data.hasDepth)
                    cmd.SetRenderTarget(data.color, data.depth);
                else
                    cmd.SetRenderTarget(data.color);

                if (data.hasFxList)
                    cmd.DrawRendererList(data.fxList);

                L2FxGpuDrawQueue.Flush(cmd);

                if (data.flushNameplates)
                {
                    cmd.SetRenderTarget(data.color);
                    L2NameplateOverlayQueue.Flush(cmd);
                }

                cmd.SetGlobalFloat(D3D9ActiveId, 0f);
            });
        }
    }
}
