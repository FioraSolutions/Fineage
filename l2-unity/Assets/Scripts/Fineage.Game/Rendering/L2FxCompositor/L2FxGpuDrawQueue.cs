using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public static partial class L2FxGpuDrawQueue
{
    public struct DrawItem
    {
        public Mesh Mesh;
        public Material Material;
        public int Submesh;
        public int Layer;
        public int RendererPriority;
        public Bounds WorldBounds;
        public ComputeBuffer SlotsBuffer;
        public Matrix4x4[] Matrices;
        public int InstanceCount;
        public MaterialPropertyBlock Properties;
    }

    static readonly List<DrawItem> s_Items = new List<DrawItem>(64);
    static bool s_Capture;
    static int s_Frame;

    public static bool IsCapturing => s_Capture;







}
