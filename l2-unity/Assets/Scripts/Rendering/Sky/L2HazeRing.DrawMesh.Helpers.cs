using System;
using UnityEngine;

public partial class L2HazeRing
{
    float ResolveSkyDistance(Camera cam)
    {
        float far = cam != null ? cam.farClipPlane : 500f;
        return Mathf.Clamp(far * 0.88f, 80f, 4000f);
    }

    void ApplyHazeLayer(GameObject go)
    {
        if (L2FxCompositorRuntime.PreferGpuQueue)
            L2FxCompositorLayers.ApplyL2HazeLayer(go);
        else
            go.layer = 0;
    }
}
