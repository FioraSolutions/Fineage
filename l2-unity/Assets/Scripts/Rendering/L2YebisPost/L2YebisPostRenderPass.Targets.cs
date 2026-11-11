using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public sealed partial class L2YebisPostRenderPass
{
    void EnsureTargets(UniversalCameraData cameraData)
    {
        RenderTextureDescriptor desc = ColorDesc(cameraData.cameraTargetDescriptor, 0, 0);

        RenderingUtils.ReAllocateIfNeeded(
            ref _temp, desc, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_L2YebisTemp");

        RenderTextureDescriptor curveDesc = ColorDesc(cameraData.cameraTargetDescriptor, 0, 0);
        curveDesc.graphicsFormat = GraphicsFormat.R16G16B16A16_SFloat;
        curveDesc.sRGB = false;
        RenderingUtils.ReAllocateIfNeeded(
            ref _sceneFloat, curveDesc, FilterMode.Point, TextureWrapMode.Clamp, name: "_L2YebisSceneFloat");

        RenderTextureDescriptor halfDesc = ColorDesc(desc, Mathf.Max(1, desc.width / 2), Mathf.Max(1, desc.height / 2));
        RenderingUtils.ReAllocateIfNeeded(
            ref _bloomA, halfDesc, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_L2YebisBloomA");
        RenderingUtils.ReAllocateIfNeeded(
            ref _bloomB, halfDesc, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_L2YebisBloomB");

        RenderTextureDescriptor luma0 = ColorDesc(desc, Mathf.Max(1, desc.width / 4), Mathf.Max(1, desc.height / 4));
        RenderingUtils.ReAllocateIfNeeded(
            ref _luma0, luma0, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_L2YebisLuma0");

        RenderTextureDescriptor luma1 = ColorDesc(desc, Mathf.Max(1, desc.width / 16), Mathf.Max(1, desc.height / 16));
        RenderingUtils.ReAllocateIfNeeded(
            ref _luma1, luma1, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_L2YebisLuma1");

        // 8x8, not 1x1: BlitCameraTexture + 1x1 RTV writes black (RenderDoc luma2/adapted = 0).
        RenderTextureDescriptor adapt = ColorDesc(desc, 8, 8);
        RenderingUtils.ReAllocateIfNeeded(
            ref _luma2, adapt, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_L2YebisLuma2");
        RenderingUtils.ReAllocateIfNeeded(
            ref _adapted, adapt, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_L2YebisAdapted");
        RenderingUtils.ReAllocateIfNeeded(
            ref _adaptedPrev, adapt, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_L2YebisAdaptedPrev");

        RenderingUtils.ReAllocateIfNeeded(
            ref _bloomExclude, desc, FilterMode.Point, TextureWrapMode.Clamp, name: "_L2YebisBloomExclude");

        if (cameraData.cameraType == CameraType.Game
            && cameraData.renderType == CameraRenderType.Base)
        {
            RenderingUtils.ReAllocateIfNeeded(
                ref _captureScene, desc, FilterMode.Point, TextureWrapMode.Clamp, name: "_L2YebisCaptureScene");
            RenderingUtils.ReAllocateIfNeeded(
                ref _captureGrade, desc, FilterMode.Point, TextureWrapMode.Clamp, name: "_L2YebisCaptureGrade");
            RenderingUtils.ReAllocateIfNeeded(
                ref _captureControl, desc, FilterMode.Point, TextureWrapMode.Clamp, name: "_L2YebisCaptureControl");
        }
    }

    static RenderTextureDescriptor ColorDesc(RenderTextureDescriptor src, int width, int height)
    {
        RenderTextureDescriptor desc = src;
        desc.msaaSamples = 1;
        desc.depthBufferBits = 0;
        desc.graphicsFormat = GraphicsFormat.R8G8B8A8_UNorm;
        desc.sRGB = false;
        desc.dimension = TextureDimension.Tex2D;
        desc.volumeDepth = 1;
        desc.useMipMap = false;
        desc.autoGenerateMips = false;
        desc.bindMS = false;
        if (width > 0)
            desc.width = width;
        if (height > 0)
            desc.height = height;
        return desc;
    }

    static Vector4 SourceScale(RTHandle source)
    {
        if (source != null && source.useScaling)
        {
            Vector4 s = source.rtHandleProperties.rtHandleScale;
            return new Vector4(s.x, s.y, 0f, 0f);
        }

        return new Vector4(1f, 1f, 0f, 0f);
    }

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
