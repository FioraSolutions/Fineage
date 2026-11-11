using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public static partial class L2NameplateOverlayQueue
{
    public static void FlushImmediateFallback()
    {
        for (int i = 0; i < s_Items.Count; i++)
            DrawNow(s_Items[i]);
        s_Items.Clear();
    }

    public static void Clear()
    {
        s_Items.Clear();
    }

    static void DrawNow(Item item)
    {
        if (item.Material == null || item.IndexBuffer == null || item.IndexCount <= 0)
            return;

        item.Material.SetBuffer("_GlyphBuffer", item.GlyphBuffer);
        var rp = new RenderParams(item.Material)
        {
            worldBounds = DrawBounds,
            camera = item.Camera,
            shadowCastingMode = ShadowCastingMode.Off,
            receiveShadows = false,
            layer = 0
        };
        Graphics.RenderPrimitivesIndexed(
            rp,
            MeshTopology.Triangles,
            item.IndexBuffer,
            item.IndexCount,
            0,
            1);
    }
}
