using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public sealed partial class L2FxCompositorRenderPass
{
    void DrawHazeRing(
        ScriptableRenderContext context,
        ref RenderingData renderingData,
        CommandBuffer cmd,
        RTHandle colorTarget,
        RTHandle cameraDepth)
    {
        int hazeMask = _settings != null ? (int)_settings.hazeLayerMask : 0;
        if (hazeMask == 0)
            return;

        Shader.SetGlobalFloat(FxGainId, 1f);
        DrawFxLayer(context, ref renderingData, cmd, colorTarget, cameraDepth, hazeMask);
        Shader.SetGlobalFloat(FxGainId, _settings != null ? _settings.fxGain : 1f);
    }

    void DrawNameplatesAfterComposite(CommandBuffer cmd, RTHandle cameraColor)
    {
        if (cmd == null || cameraColor == null)
        {
            L2NameplateOverlayQueue.Clear();
            return;
        }

        CoreUtils.SetRenderTarget(cmd, cameraColor, ClearFlag.None, Color.clear);
        L2NameplateOverlayQueue.Flush(cmd);
    }

    void BlitDebug(CommandBuffer cmd, RTHandle source, RTHandle cameraColor)
    {
        _postMaterial.SetFloat("_TransferMode", _settings.decodeSrgbToLinear ? 2f : 0f);
        Blitter.BlitCameraTexture(cmd, source, cameraColor, _postMaterial, PostDebugPass);
    }

    static void SetBlitTexelSize(Material material, RTHandle source)
    {
        RenderTexture rt = source != null ? source.rt : null;
        if (rt == null)
            return;

        material.SetVector(
            KawaseTexelSizeId,
            new Vector4(1f / rt.width, 1f / rt.height, rt.width, rt.height));
    }
}
