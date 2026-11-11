using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public static partial class L2NameplateOverlayQueue
{
    public static void Enqueue(
        Material material,
        GraphicsBuffer glyphBuffer,
        GraphicsBuffer indexBuffer,
        int indexCount,
        Camera camera)
    {
        if (material == null || glyphBuffer == null || indexBuffer == null || indexCount <= 0)
            return;

        s_Items.Add(new Item
        {
            Material = material,
            GlyphBuffer = glyphBuffer,
            IndexBuffer = indexBuffer,
            IndexCount = indexCount,
            Camera = camera
        });
    }

    public static void Flush(CommandBuffer cmd)
    {
        if (cmd == null || s_Items.Count == 0)
        {
            s_Items.Clear();
            return;
        }

        for (int i = 0; i < s_Items.Count; i++)
        {
            Item item = s_Items[i];
            if (item.Material == null || item.IndexBuffer == null || item.IndexCount <= 0)
                continue;

            item.Material.SetBuffer("_GlyphBuffer", item.GlyphBuffer);
            cmd.DrawProcedural(
                item.IndexBuffer,
                Matrix4x4.identity,
                item.Material,
                0,
                MeshTopology.Triangles,
                item.IndexCount,
                1);
        }

        s_Items.Clear();
    }
}
