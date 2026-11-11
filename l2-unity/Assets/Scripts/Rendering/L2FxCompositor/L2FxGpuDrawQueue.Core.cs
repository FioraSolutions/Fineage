using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public static partial class L2FxGpuDrawQueue
{
    public static void BeginFrameCapture()
    {
        int frame = Time.frameCount;
        if (s_Frame != frame)
        {
            s_Items.Clear();
            s_Frame = frame;
        }

        s_Capture = true;
    }

    public static void EndFrameCapture()
    {
        s_Capture = false;
    }

    public static void Enqueue(DrawItem item)
    {
        if (item.Mesh == null || item.Material == null || item.InstanceCount <= 0)
            return;
        s_Items.Add(item);
    }
}
