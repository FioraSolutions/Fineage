using UnityEngine;

public sealed partial class ItemDropLayerService
{
    public void ApplyDropItemLayer(GameObject root)
    {
        int layer = Layer;
        if (layer < 0 || root == null)
            return;
        SetLayerSkipFx(root, layer);
        DisablePhysicalDropMeshColliders(root.transform);
    }

    public void ApplyIgnoreRaycastLayer(GameObject root)
    {
        if (root == null)
            return;
        int layer = LayerMask.NameToLayer("Ignore Raycast");
        if (layer < 0)
            layer = LayerMask.NameToLayer("SkillEffect");
        if (layer < 0)
            return;
        SetLayerAll(root, layer);
    }

    public bool IsFxTransform(Transform t)
    {
        while (t != null)
        {
            if (IsFxNode(t.name))
                return true;
            t = t.parent;
        }

        return false;
    }
}
