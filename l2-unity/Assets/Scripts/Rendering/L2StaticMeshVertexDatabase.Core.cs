using System;
using UnityEngine;

public static partial class L2StaticMeshVertexDatabase
{
    public static MapData Parse(TextAsset asset)
    {
        if (asset == null)
            throw new ArgumentNullException(nameof(asset));
        MapData data = JsonUtility.FromJson<MapData>(asset.text);
        if (data == null || data.meshes == null)
            throw new InvalidOperationException("Invalid static-mesh vertex database: " + asset.name);
        return data;
    }
}
