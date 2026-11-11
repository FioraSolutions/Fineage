using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public sealed partial class L2FxCompositorRenderPass
{
    void ExecuteDirect(
        ref ScriptableRenderContext context,
        ref RenderingData renderingData)
    {
        if (!ShouldRun(ref renderingData))
        {
            L2FxGpuDrawQueue.FlushImmediateFallback();
            if (!L2YebisPostRuntime.WillFlushNameplates)
                L2NameplateOverlayQueue.FlushImmediateFallback();
            Shader.SetGlobalFloat(D3D9ActiveId, 0f);
            return;
        }

        RTHandle cameraColor = renderingData.cameraData.renderer.cameraColorTargetHandle;
        RTHandle cameraDepth = renderingData.cameraData.renderer.cameraDepthTargetHandle;
        if (cameraColor == null)
        {
            L2FxGpuDrawQueue.FlushImmediateFallback();
            if (!L2YebisPostRuntime.WillFlushNameplates)
                L2NameplateOverlayQueue.FlushImmediateFallback();
            return;
        }

        CommandBuffer cmd = CommandBufferPool.Get("L2 FX Direct UNORM");
        using (new ProfilingScope(SamplerDirect))
        {
            cmd.SetGlobalFloat(D3D9ActiveId, 1f);
            cmd.SetGlobalFloat(DebugModeId, _settings.debugMode);
            cmd.SetGlobalFloat(FxGainId, 1f);
            DrawSkillFx(
                context,
                ref renderingData,
                cmd,
                cameraColor,
                cameraDepth,
                clearFx: false);
            if (!L2YebisPostRuntime.WillFlushNameplates)
                DrawNameplatesAfterComposite(cmd, cameraColor);
            cmd.SetGlobalFloat(D3D9ActiveId, 0f);
        }

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    void ExecuteLegacyComposite(
        ScriptableRenderContext context,
        ref RenderingData renderingData,
        CommandBuffer cmd,
        RTHandle cameraColor,
        RTHandle cameraDepth)
    {
        _transferMaterial.SetFloat("_TransferMode", _settings.encodeLinearToSrgb ? 1f : 0f);
        Blitter.BlitCameraTexture(cmd, cameraColor, _l2Color, _transferMaterial, 0);

        DrawCelestialDiscs(context, ref renderingData, cmd, _l2Color, cameraDepth, "legacy", _l2Color);
        DrawCloudSheets(context, ref renderingData, cmd, _l2Color, cameraDepth);
        DrawHazeRing(context, ref renderingData, cmd, _l2Color, cameraDepth);
        DrawSkillFx(context, ref renderingData, cmd, _l2Color, cameraDepth, clearFx: false);
        DecodeToCamera(cmd, _l2Color, cameraColor);
    }

    void ExecuteSkillPost(
        ScriptableRenderContext context,
        ref RenderingData renderingData,
        CommandBuffer cmd,
        RTHandle cameraColor,
        RTHandle cameraDepth)
    {
        // 1) Encode camera → UNORM working buffer (same path that already matched L2).
        _transferMaterial.SetFloat("_TransferMode", _settings.encodeLinearToSrgb ? 1f : 0f);
        Blitter.BlitCameraTexture(cmd, cameraColor, _l2Color, _transferMaterial, 0);

        // 2) Snapshot the colour pass (sky+world, A=1), then blend clouds onto it —
        // same as original: Tex Before is the sky, then SrcA/InvSrcA.
        cmd.CopyTexture(_l2Color, _l2Scene);
        DrawCloudSheets(context, ref renderingData, cmd, _l2Color, cameraDepth);

        // 3) Snapshot again so clouds stay in the scene copy (no fxGain/bloom).
        // Haze still draws after and covers the horizon belt.
        cmd.CopyTexture(_l2Color, _l2Scene);
        DrawHazeRing(context, ref renderingData, cmd, _l2Color, cameraDepth);
        DrawSkillFx(context, ref renderingData, cmd, _l2Color, cameraDepth, clearFx: false);

        BindPostParams();
        _postMaterial.SetTexture(SceneTexId, _l2Scene != null ? _l2Scene.rt : null);

        // 3) Bloom from what skills added above the scene (not from darken).
        Blitter.BlitCameraTexture(cmd, _l2Color, _l2BloomA, _postMaterial, PostExtractPass);

        _postMaterial.SetFloat(KawaseOffsetId, 1f);
        SetBlitTexelSize(_postMaterial, _l2BloomA);
        Blitter.BlitCameraTexture(cmd, _l2BloomA, _l2BloomB, _postMaterial, PostKawasePass);

        _postMaterial.SetFloat(KawaseOffsetId, 2f);
        SetBlitTexelSize(_postMaterial, _l2BloomB);
        Blitter.BlitCameraTexture(cmd, _l2BloomB, _l2BloomA, _postMaterial, PostKawasePass);

        if (_settings.debugMode == 2)
        {
            BlitDebug(cmd, _l2Scene, cameraColor);
            return;
        }

        if (_settings.debugMode == 3)
        {
            BlitDebug(cmd, _l2Color, cameraColor);
            return;
        }

        if (_settings.debugMode == 4)
        {
            BlitDebug(cmd, _l2BloomA, cameraColor);
            return;
        }

        // 4) RGB Boost on skill/sun/moon delta. Sky stays in the scene copy (no extra dest).
        DrawCelestialDiscs(context, ref renderingData, cmd, _l2Color, cameraDepth, "skillPost", _l2Scene);

        _postMaterial.SetTexture(BloomTexId, _l2BloomA != null ? _l2BloomA.rt : null);
        _postMaterial.SetFloat(
            "_TransferMode",
            _settings.debugMode == 1
                ? 3f
                : (_settings.decodeSrgbToLinear ? 2f : 0f));
        Blitter.BlitCameraTexture(cmd, _l2Color, cameraColor, _postMaterial, PostCompositePass);
    }

    static void BindUnormColorKeepContents(CommandBuffer cmd, RTHandle colorTarget, RTHandle cameraDepth)
    {
        if (colorTarget == null)
            return;

        if (cameraDepth != null)
        {
            cmd.SetRenderTarget(
                colorTarget,
                RenderBufferLoadAction.Load,
                RenderBufferStoreAction.Store,
                cameraDepth,
                RenderBufferLoadAction.Load,
                RenderBufferStoreAction.Store);
            return;
        }

        cmd.SetRenderTarget(colorTarget, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store);
    }

    void DecodeToCamera(CommandBuffer cmd, RTHandle source, RTHandle cameraColor)
    {
        _transferMaterial.SetFloat("_TransferMode", _settings.decodeSrgbToLinear ? 2f : 0f);
        Blitter.BlitCameraTexture(cmd, source, cameraColor, _transferMaterial, 1);

        if (_settings.debugMode == 1)
        {
            _transferMaterial.SetFloat("_TransferMode", 3f);
            Blitter.BlitCameraTexture(cmd, source, cameraColor, _transferMaterial, 1);
        }
    }

    void BindPostParams()
    {
        _postMaterial.SetFloat(BloomThresholdId, _settings.bloomThreshold);
        _postMaterial.SetFloat(BloomPreGainId, 1f);
        _postMaterial.SetFloat(BloomIntensityId, _settings.bloomIntensity);
        _postMaterial.SetFloat(FxGainId, _settings.fxGain);
        Shader.SetGlobalFloat(FxGainId, _settings.fxGain);
    }

    bool WantSkillPost()
    {
        return _settings != null && _settings.enableSkillPost && _postMaterial != null;
    }

    bool UseSkillPost()
    {
        return WantSkillPost() && _l2Scene != null && _l2BloomA != null && _l2BloomB != null;
    }

    bool ShouldRun(ref RenderingData renderingData)
    {
        return ShouldRun(renderingData.cameraData.camera);
    }

    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        Shader.SetGlobalFloat(D3D9ActiveId, 0f);
    }

    bool ShouldRun(Camera camera)
    {
        if (_settings == null || !_settings.enableD3D9Compositor)
            return false;

        CameraType type = camera.cameraType;
        if (_settings.gameCameraOnly &&
            type != CameraType.Game &&
            type != CameraType.VR)
        {
            return false;
        }

        return true;
    }
}
