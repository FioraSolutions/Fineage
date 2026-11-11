using UnityEngine;

public sealed partial class ItemDropLayerService
{
    static bool IsFxNode(string name)
    {
        if (string.IsNullOrEmpty(name))
            return false;
        return name.IndexOf("HitPointProxy", System.StringComparison.OrdinalIgnoreCase) >= 0
            || name.IndexOf("e_u056", System.StringComparison.OrdinalIgnoreCase) >= 0
            || name.IndexOf("DropGroundGlow", System.StringComparison.OrdinalIgnoreCase) >= 0
            || name.IndexOf("Coin_RuntimeClone", System.StringComparison.OrdinalIgnoreCase) >= 0;
    }

    void SetLayerSkipFx(GameObject go, int layer)
    {
        if (go == null)
            return;
        if (IsFxNode(go.name))
        {
            ApplyIgnoreRaycastLayer(go);
            return;
        }

        go.layer = layer;
        Transform t = go.transform;
        for (int i = 0; i < t.childCount; i++)
            SetLayerSkipFx(t.GetChild(i).gameObject, layer);
    }

    static void SetLayerAll(GameObject go, int layer)
    {
        go.layer = layer;
        Transform t = go.transform;
        for (int i = 0; i < t.childCount; i++)
            SetLayerAll(t.GetChild(i).gameObject, layer);
    }

    static void DisablePhysicalDropMeshColliders(Transform root)
    {
        Transform dropMesh = root.Find("DropMesh");
        if (dropMesh == null)
            return;

        Collider[] colliders = dropMesh.GetComponentsInChildren<Collider>(true);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] != null)
                colliders[i].enabled = false;
        }
    }
}
