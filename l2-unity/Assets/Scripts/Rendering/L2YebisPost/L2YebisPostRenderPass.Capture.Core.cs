using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public sealed partial class L2YebisPostRenderPass
{
    string CopyCapture(CommandBuffer cmd, RTHandle source, RTHandle destination)
    {
        RenderTexture src = source != null ? source.rt : null;
        RenderTexture dst = destination != null ? destination.rt : null;
        if (src != null && dst != null
            && src.width == dst.width
            && src.height == dst.height
            && src.graphicsFormat == dst.graphicsFormat
            && src.antiAliasing <= 1
            && dst.antiAliasing <= 1)
        {
            cmd.SetRenderTarget(_captureControl.nameID);
            cmd.CopyTexture(src, 0, 0, dst, 0, 0);
            // Debug.Log("[Yebis] CopyTexture " + src.width + "x" + src.height + " " + src.graphicsFormat);
            return "CopyTexture";
        }

        // Debug.LogWarning("[Yebis] CopyTexture пропущен src=" + RtSize(source) + " dst=" + RtSize(destination));
        BlitFull(cmd, source, destination, PassCopy);
        return "Blit";
    }
}
