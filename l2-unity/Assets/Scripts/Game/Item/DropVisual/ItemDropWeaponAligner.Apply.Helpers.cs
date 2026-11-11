using UnityEngine;

public sealed partial class ItemDropWeaponAligner
{
    static bool TryGetBladeMarkers(Transform root, out Transform bladeBase, out Transform bladeTip)
    {
        bladeBase = null;
        bladeTip = null;
        if (root == null)
            return false;

        Transform[] all = root.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < all.Length; i++)
        {
            string name = all[i].name;
            if (IsBladeTipName(name))
                bladeTip = all[i];
            else if (IsBladeBaseName(name))
                bladeBase = all[i];
        }

        return bladeBase != null && bladeTip != null && bladeBase != bladeTip;
    }

    static bool IsBladeTipName(string name)
    {
        return name.EndsWith("Tip", System.StringComparison.OrdinalIgnoreCase) ||
               name.IndexOf("_Tip", System.StringComparison.OrdinalIgnoreCase) >= 0;
    }

    static bool IsBladeBaseName(string name)
    {
        return name.EndsWith("Base", System.StringComparison.OrdinalIgnoreCase) ||
               name.IndexOf("_Base", System.StringComparison.OrdinalIgnoreCase) >= 0;
    }

    static float LongestMeshAxis(GameObject go)
    {
        float longest = 0f;
        MeshFilter[] filters = go.GetComponentsInChildren<MeshFilter>(true);
        for (int i = 0; i < filters.Length; i++)
        {
            Mesh mesh = filters[i].sharedMesh;
            if (mesh == null)
                continue;
            Vector3 size = Vector3.Scale(mesh.bounds.size, filters[i].transform.lossyScale);
            longest = Mathf.Max(longest, size.x, size.y, size.z);
        }

        SkinnedMeshRenderer[] skins = go.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        for (int i = 0; i < skins.Length; i++)
        {
            Mesh mesh = skins[i].sharedMesh;
            if (mesh == null)
                continue;
            Vector3 size = Vector3.Scale(mesh.bounds.size, skins[i].transform.lossyScale);
            longest = Mathf.Max(longest, size.x, size.y, size.z);
        }

        return longest;
    }
}
