using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public sealed partial class L2YebisPostRenderPass
{
    static string RtSize(RTHandle handle)
    {
        RenderTexture rt = handle != null ? handle.rt : null;
        if (rt == null)
            return "null";
        return rt.width + "x" + rt.height + " " + rt.graphicsFormat + " msaa=" + rt.antiAliasing;
    }

    void BlitFull(CommandBuffer cmd, RTHandle source, RTHandle destination, int pass)
    {
        if (source == null || destination == null)
            return;

        RenderTexture destRt = destination.rt;
        int w = destRt != null ? destRt.width : 8;
        int h = destRt != null ? destRt.height : 8;
        Vector4 scale = SourceScale(source);
        _material.SetVector(BlitScaleBiasId, scale);
        cmd.SetGlobalVector(BlitScaleBiasId, scale);
        cmd.SetGlobalTexture(BlitTextureId, source.nameID);
        cmd.SetRenderTarget(destination.nameID);
        cmd.SetViewport(new Rect(0f, 0f, w, h));
        // No MaterialPropertyBlock: Blitter's MPB drops material textures
        // (_AdaptedTex → black), so Adapt was lerp(0, luma, 0.08).
        cmd.DrawProcedural(Matrix4x4.identity, _material, pass, MeshTopology.Triangles, 3, 1);
    }

    void SetBlitTexelSize(RTHandle source)
    {
        RenderTexture rt = source != null ? source.rt : null;
        if (rt == null)
            return;

        _material.SetVector(
            KawaseTexelSizeId,
            new Vector4(1f / rt.width, 1f / rt.height, rt.width, rt.height));
    }

    static string RtInfo(RTHandle handle)
    {
        if (handle == null)
            return "null";
        if (handle.rt == null)
            return "rt=null";
        return $"{handle.rt.width}x{handle.rt.height}";
    }
}
