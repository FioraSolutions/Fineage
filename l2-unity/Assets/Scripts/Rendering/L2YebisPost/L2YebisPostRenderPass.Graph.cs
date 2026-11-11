using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public sealed partial class L2YebisPostRenderPass
{
    public L2YebisPostRenderPass(L2FxCompositorSettings settings, Material material)
    {
        _settings = settings;
        _material = material;
        renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public void Dispose()
    {
        _temp?.Release();
        _temp = null;
        _bloomA?.Release();
        _bloomA = null;
        _bloomB?.Release();
        _bloomB = null;
        _luma0?.Release();
        _luma0 = null;
        _luma1?.Release();
        _luma1 = null;
        _luma2?.Release();
        _luma2 = null;
        _adapted?.Release();
        _adapted = null;
        _adaptedPrev?.Release();
        _adaptedPrev = null;
        _sceneFloat?.Release();
        _sceneFloat = null;
        _bloomExclude?.Release();
        _bloomExclude = null;
        _captureScene?.Release();
        _captureScene = null;
        _captureGrade?.Release();
        _captureGrade = null;
        _captureControl?.Release();
        _captureControl = null;
        CoreUtils.Destroy(_excludeSkyMaterial);
        _excludeSkyMaterial = null;
        _adaptedSeeded = false;
        _execFrame = 0;
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        EnsureTargets(renderingData.cameraData);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (_material == null)
        {
            L2NameplateOverlayQueue.FlushImmediateFallback();
            L2YebisPostRuntime.WillFlushNameplates = false;
            return;
        }

        EnsureTargets(renderingData.cameraData);
        if (_temp == null)
        {
            L2NameplateOverlayQueue.FlushImmediateFallback();
            L2YebisPostRuntime.WillFlushNameplates = false;
            return;
        }

        RTHandle cameraColor = renderingData.cameraData.renderer.cameraColorTargetHandle;
        if (cameraColor == null)
        {
            L2NameplateOverlayQueue.FlushImmediateFallback();
            L2YebisPostRuntime.WillFlushNameplates = false;
            return;
        }

        CommandBuffer cmd = CommandBufferPool.Get("L2 Yebis Post");
        using (new ProfilingScope(cmd, Sampler))
            ExecuteYebis(cmd, cameraColor, renderingData.cameraData, renderingData.cameraData.renderer.cameraDepthTargetHandle);
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
        L2YebisPostRuntime.WillFlushNameplates = false;
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        if (_material == null)
        {
            L2NameplateOverlayQueue.FlushImmediateFallback();
            L2YebisPostRuntime.WillFlushNameplates = false;
            return;
        }

        UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
        UniversalResourceData resources = frameData.Get<UniversalResourceData>();
        EnsureTargets(cameraData);

        TextureHandle color = resources.activeColorTexture;
        if (!color.IsValid() || _temp == null)
        {
            L2NameplateOverlayQueue.FlushImmediateFallback();
            L2YebisPostRuntime.WillFlushNameplates = false;
            return;
        }

        TextureHandle depth = resources.activeDepthTexture;
        using (IUnsafeRenderGraphBuilder builder =
               renderGraph.AddUnsafePass<YebisPassData>("L2 Yebis Post", out YebisPassData passData, Sampler))
        {
            passData.color = color;
            passData.depth = depth;
            passData.hasDepth = depth.IsValid();
            passData.pass = this;
            passData.cameraData = cameraData;

            builder.UseTexture(color, AccessFlags.ReadWrite);
            if (passData.hasDepth)
                builder.UseTexture(depth, AccessFlags.ReadWrite);
            // Persistent working buffers are written via RTHandle fields inside ExecuteYebis.
            builder.AllowPassCulling(false);
            builder.AllowGlobalStateModification(true);
            builder.SetRenderFunc(static (YebisPassData data, UnsafeGraphContext context) =>
            {
                CommandBuffer cmd = CommandBufferHelpers.GetNativeCommandBuffer(context.cmd);
                RTHandle depthRt = data.hasDepth ? (RTHandle)data.depth : null;
                data.pass.ExecuteYebis(cmd, data.color, data.cameraData, depthRt);
            });
        }

        L2YebisPostRuntime.WillFlushNameplates = false;
    }
}
