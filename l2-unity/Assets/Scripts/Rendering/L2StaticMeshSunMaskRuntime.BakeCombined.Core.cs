using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public sealed partial class L2StaticMeshSunMaskRuntime
{
    static bool TryAddCombinedVisibility(
        MeshFilter filter,
        byte[] visibility,
        Dictionary<Mesh, CombinedStreamData> streams,
        out string error)
    {
        Mesh mesh = filter.sharedMesh;
        if (!TryGetCombinedVertexRange(
            filter,
            visibility.Length,
            out int firstVertex,
            out error))
            return false;

        if (!streams.TryGetValue(mesh, out CombinedStreamData stream))
        {
            stream = new CombinedStreamData(mesh);
            streams.Add(mesh, stream);
        }
        for (int i = 0; i < visibility.Length; i++)
            stream.packed[firstVertex + i] = visibility[i] / 255f;
        if (!stream.filters.Contains(filter))
            stream.filters.Add(filter);
        error = null;
        return true;
    }
}
