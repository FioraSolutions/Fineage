using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public sealed partial class L2StaticMeshSunMaskRuntime
{
    static bool TryGetCombinedVertexRange(
        MeshFilter filter,
        int expectedVertexCount,
        out int firstVertex,
        out string error)
    {
        MeshRenderer renderer = filter.GetComponent<MeshRenderer>();
        Mesh mesh = filter.sharedMesh;
        firstVertex = 0;
        if (renderer == null || mesh == null || !renderer.isPartOfStaticBatch)
        {
            error = "not a static batch renderer";
            return false;
        }

        int subMeshStart = renderer.subMeshStartIndex;
        int subMeshCount = renderer.sharedMaterials.Length;
        int rangeStart = int.MaxValue;
        int rangeEnd = 0;
        for (int i = 0; i < subMeshCount; i++)
        {
            int subMesh = subMeshStart + i;
            if (subMesh < 0 || subMesh >= mesh.subMeshCount)
                continue;
            SubMeshDescriptor descriptor = mesh.GetSubMesh(subMesh);
            rangeStart = Mathf.Min(rangeStart, descriptor.firstVertex);
            rangeEnd = Mathf.Max(
                rangeEnd,
                descriptor.firstVertex + descriptor.vertexCount);
        }

        int rangeLength = rangeStart == int.MaxValue ? 0 : rangeEnd - rangeStart;
        if (rangeLength != expectedVertexCount)
        {
            error = "range=" + rangeLength +
                " submeshes=" + subMeshStart + "+" + subMeshCount;
            return false;
        }

        firstVertex = rangeStart;
        error = null;
        return true;
    }

    static void InstallCombinedStreams(Dictionary<Mesh, CombinedStreamData> streams)
    {
        foreach (CombinedStreamData data in streams.Values)
        {
            Mesh stream = CreateStream(data.mesh, data.packed);
            for (int i = 0; i < data.filters.Count; i++)
            {
                MeshFilter filter = data.filters[i];
                L2StaticMeshSunMaskRuntime component =
                    filter.gameObject.AddComponent<L2StaticMeshSunMaskRuntime>();
                component.Attach(filter, stream, i == 0);
            }
        }
    }
}
