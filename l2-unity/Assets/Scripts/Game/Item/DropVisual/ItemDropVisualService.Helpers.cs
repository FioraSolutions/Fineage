using UnityEngine;

public sealed partial class ItemDropVisualService
{
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

        return longest;
    }

    static void ClearChildren(Transform root)
    {
        for (int i = root.childCount - 1; i >= 0; i--)
            Object.Destroy(root.GetChild(i).gameObject);
    }
}
