using System;
using UnityEngine;

public static partial class L2StaticMeshSunMaskDatabase
{
    public static MapData Parse(TextAsset asset)
    {
        if (asset == null)
            throw new ArgumentNullException(nameof(asset));
        MapData data = JsonUtility.FromJson<MapData>(asset.text);
        if (data == null || data.actors == null)
            throw new InvalidOperationException("Invalid static-mesh mask database: " + asset.name);
        return data;
    }
}
