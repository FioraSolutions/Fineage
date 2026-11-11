using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public static partial class L2FxGpuDrawQueue
{
    public static void FlushImmediateFallback()
    {
        // Compositor disabled: draw now like the old LateUpdate path.
        for (int i = 0; i < s_Items.Count; i++)
            DrawItemNow(s_Items[i]);
        s_Items.Clear();
        s_Capture = false;
    }

    public static void Flush(CommandBuffer cmd)
    {
        if (cmd == null || s_Items.Count == 0)
        {
            s_Items.Clear();
            return;
        }

        s_Items.Sort((a, b) => a.RendererPriority.CompareTo(b.RendererPriority));

        for (int i = 0; i < s_Items.Count; i++)
        {
            DrawItem item = s_Items[i];
            if (item.Mesh == null || item.Material == null || item.InstanceCount <= 0)
                continue;

            // Must use CommandBuffer so draws hit the UNORM target set by the pass.
            cmd.DrawMeshInstanced(
                item.Mesh,
                item.Submesh,
                item.Material,
                0,
                item.Matrices,
                item.InstanceCount,
                item.Properties);
        }

        s_Items.Clear();
    }

    static void DrawItemNow(DrawItem item)
    {
        var rp = new RenderParams(item.Material)
        {
            worldBounds = item.WorldBounds,
            layer = item.Layer,
            rendererPriority = item.RendererPriority,
            shadowCastingMode = ShadowCastingMode.Off,
            receiveShadows = false,
            matProps = item.Properties
        };
        Graphics.RenderMeshInstanced(rp, item.Mesh, item.Submesh, item.Matrices, item.InstanceCount);
    }

    public static void Clear()
    {
        s_Items.Clear();
        s_Capture = false;
    }
}
