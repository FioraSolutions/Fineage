using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public sealed partial class L2YebisPostRenderPass
{
    void MaybeLogYebis(CommandBuffer cmd, RTHandle cameraColor)
    {
        _execFrame++;
        if (_execFrame > 5 && (_execFrame % 120) != 0)
            return;

        Vector4 camScale = SourceScale(cameraColor);
        // Debug.Log(
            // $"[Yebis] f={_execFrame} debug={_settings.yebisDebugMode} mix={_settings.yebisContrastMix:F2} " +
            // $"camRt={(cameraColor != null && cameraColor.rt != null)} useScaling={(cameraColor != null && cameraColor.useScaling)} " +
            // $"camScale=({camScale.x:F3},{camScale.y:F3}) " +
            // $"luma0={RtInfo(_luma0)} luma1={RtInfo(_luma1)} luma2={RtInfo(_luma2)} " +
            // $"adapted={RtInfo(_adapted)} prev={RtInfo(_adaptedPrev)} " +
            // $"adaptLerp={Mathf.Clamp01(_settings.yebisAdaptationSensitivity * 0.16f):F3} seeded={_adaptedSeeded}");

        QueueReadback(cmd, _luma0, "luma0");
        QueueReadback(cmd, _luma1, "luma1");
        QueueReadback(cmd, _luma2, "luma2");
        QueueReadback(cmd, _adapted, "adapted");
        QueueReadback(cmd, _adaptedPrev, "adaptedPrev");
    }

    void QueueReadback(CommandBuffer cmd, RTHandle handle, string tag)
    {
        if (handle == null || handle.rt == null)
        {
            // Debug.Log($"[Yebis] {tag} skip rt=null f={_execFrame}");
            return;
        }

        RenderTexture rt = handle.rt;
        int width = rt.width;
        int height = rt.height;
        int frame = _execFrame;
        cmd.RequestAsyncReadback(rt, request =>
        {
            // RTHandles are released/reallocated before this callback (Dispose, resize).
            // Never touch the captured RenderTexture here.
            if (request.hasError)
            {
                Debug.LogWarning($"[Yebis] {tag} readback FAIL f={frame} {width}x{height}");
                return;
            }

            var bytes = request.GetData<byte>();
            int pixels = width * height;
            int stride = pixels > 0 ? bytes.Length / pixels : 0;
            if (stride < 1 || bytes.Length < 4)
            {
                //Debug.Log($"[Yebis] {tag} f={frame} bytes={bytes.Length} {width}x{height}");
                return;
            }

            int r0 = bytes[0];
            int g0 = bytes[1];
            int b0 = bytes[2];
            int a0 = bytes[3];
            int mid = (pixels / 2) * stride;
            int rMid = bytes[mid];
            long sum = 0;
            int n = Mathf.Min(pixels, 64);
            for (int i = 0; i < n; i++)
                sum += bytes[i * stride];
            float avg = sum / (n * 255f);
            //Debug.Log(
             //   $"[Yebis] {tag} f={frame} {width}x{height} stride={stride} " +
            //    $"p0=({r0},{g0},{b0},{a0}) midR={rMid} avgR={avg:F3}");
        });
    }
}
