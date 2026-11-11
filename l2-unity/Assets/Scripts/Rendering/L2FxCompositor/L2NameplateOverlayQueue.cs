using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public static partial class L2NameplateOverlayQueue
{
    struct Item
    {
        public Material Material;
        public GraphicsBuffer GlyphBuffer;
        public GraphicsBuffer IndexBuffer;
        public int IndexCount;
        public Camera Camera;
    }

    static readonly List<Item> s_Items = new List<Item>(8);
    static readonly Bounds DrawBounds = new Bounds(Vector3.zero, Vector3.one * 100000f);





}
