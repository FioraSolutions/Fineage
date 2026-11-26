using UnityEngine;

/// <summary>
/// Layer mask for camera→head nameplate occlusion (world geometry + character bodies).
/// </summary>
public static class L2NameplateOcclusionMask
{
    public static readonly string[] RequiredLayerNames =
    {
        "Terrain",
        "StaticMesh",
        "StaticMeshBuildings",
        "Brush",
        "Obstacle",
        "Entity",
        "EntityClick",
        "Player",
    };

    static bool _missingLayerLogged;

    public static bool TryGetMask(out LayerMask mask)
    {
        mask = 0;
        for (int i = 0; i < RequiredLayerNames.Length; i++)
        {
            string layerName = RequiredLayerNames[i];
            int layer = LayerMask.NameToLayer(layerName);
            if (layer < 0)
            {
                if (!_missingLayerLogged)
                {
                    _missingLayerLogged = true;
                    FineageLog.Error(
                        FineageLogCategory.World,
                        "L2NameplateOcclusionMask: missing Physics layer '" + layerName
                        + "'. Nameplate occlusion is disabled until ProjectSettings/TagManager defines it.");
                }

                return false;
            }

            mask |= 1 << layer;
        }

        return true;
    }
}