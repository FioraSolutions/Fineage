using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public sealed partial class L2FxCompositorRenderPass
{
    void DrawCelestialDiscs(
        ScriptableRenderContext context,
        ref RenderingData renderingData,
        CommandBuffer cmd,
        RTHandle colorTarget,
        RTHandle cameraDepth,
        string path,
        RTHandle encodedScene)
    {
        float gain = _settings != null ? _settings.fxGain : 0f;
        float start = _settings != null ? _settings.skyBoostStart : 0f;
        float full = _settings != null ? _settings.skyBoostFull : 0f;
        if (full <= start + 0.001f)
        {
            start = 0.30f;
            full = 0.50f;
        }

        Shader.SetGlobalFloat(FxGainId, 1f);

        int celestialMask = _settings != null ? (int)_settings.celestialLayerMask : 0;
        if (celestialMask != 0)
            DrawFxLayer(context, ref renderingData, cmd, colorTarget, cameraDepth, celestialMask);

        // L2FxSkyDebug.LogCpuAndEnqueueGpu(
            // cmd,
            // path,
            // WantCelestial(),
            // _settings != null && _settings.includeSkyInBoost,
            // false,
            // "camera skybox (UNORM DrawSkybox was empty)",
            // encodedScene,
            // colorTarget,
            // renderingData.cameraData.camera,
            // gain,
            // start,
            // full);
        Shader.SetGlobalFloat(FxGainId, gain);
    }

    void DrawSkillFx(
        ScriptableRenderContext context,
        ref RenderingData renderingData,
        CommandBuffer cmd,
        RTHandle colorTarget,
        RTHandle cameraDepth,
        bool clearFx)
    {
        ClearFlag clear = clearFx ? ClearFlag.Color : ClearFlag.None;
        if (_settings.bindCameraDepth && cameraDepth != null)
            CoreUtils.SetRenderTarget(cmd, colorTarget, cameraDepth, clear, Color.clear);
        else
            CoreUtils.SetRenderTarget(cmd, colorTarget, clear, Color.clear);

        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();

        DrawingSettings drawing = CreateDrawingSettings(
            ForwardOnlyTag,
            ref renderingData,
            SortingCriteria.SortingLayer | SortingCriteria.RenderQueue);
        drawing.perObjectData = PerObjectData.None;

        context.DrawRenderers(renderingData.cullResults, ref drawing, ref _filtering);

        L2FxGpuDrawQueue.Flush(cmd);
        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();
    }

    void DrawFxLayer(
        ScriptableRenderContext context,
        ref RenderingData renderingData,
        CommandBuffer cmd,
        RTHandle colorTarget,
        RTHandle cameraDepth,
        int layerMask)
    {
        if (layerMask == 0)
            return;

        BindUnormColorKeepContents(
            cmd,
            colorTarget,
            _settings.bindCameraDepth ? cameraDepth : null);

        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();

        var filtering = new FilteringSettings(RenderQueueRange.transparent, layerMask);
        DrawingSettings drawing = CreateDrawingSettings(
            ForwardOnlyTag,
            ref renderingData,
            SortingCriteria.SortingLayer | SortingCriteria.RenderQueue);
        drawing.perObjectData = PerObjectData.None;
        context.DrawRenderers(renderingData.cullResults, ref drawing, ref filtering);
    }

    void DrawCloudSheets(
        ScriptableRenderContext context,
        ref RenderingData renderingData,
        CommandBuffer cmd,
        RTHandle colorTarget,
        RTHandle cameraDepth)
    {
        int cloudMask = _settings != null ? (int)_settings.cloudLayerMask : 0;
        if (cloudMask == 0)
            return;

        Shader.SetGlobalFloat(FxGainId, 1f);
        DrawFxLayer(context, ref renderingData, cmd, colorTarget, cameraDepth, cloudMask);
    }
}
