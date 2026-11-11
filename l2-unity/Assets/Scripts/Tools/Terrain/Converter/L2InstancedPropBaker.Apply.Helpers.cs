#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static partial class L2InstancedPropBaker
{
    private static void ReparentKeepers(Component[] components, Transform keep)
    {
        for (int i = 0; i < components.Length; i++)
        {
            if (components[i] == null)
            {
                continue;
            }

            Transform t = components[i].transform;
            t.SetParent(keep, true);
            MeshFilter filter = t.GetComponent<MeshFilter>();
            MeshRenderer meshRenderer = t.GetComponent<MeshRenderer>();
            if (filter != null)
            {
                Object.DestroyImmediate(filter);
            }

            if (meshRenderer != null)
            {
                Object.DestroyImmediate(meshRenderer);
            }
        }
    }

    public static L2InstancedPropSet BakeFromRoot(GameObject root, string assetPath, bool castShadows)
    {
        if (root == null)
        {
            // Debug.LogError("L2InstancedPropBaker: root is null");
            return null;
        }

        MeshRenderer[] renderers = root.GetComponentsInChildren<MeshRenderer>(true);
        var groups = new Dictionary<long, List<Matrix4x4>>();
        var meshes = new Dictionary<long, Mesh>();
        var materials = new Dictionary<long, Material>();
        Matrix4x4 rootInverse = root.transform.worldToLocalMatrix;
        int skipped = 0;

        for (int i = 0; i < renderers.Length; i++)
        {
            MeshRenderer meshRenderer = renderers[i];
            MeshFilter filter = meshRenderer.GetComponent<MeshFilter>();
            if (filter == null || filter.sharedMesh == null || meshRenderer.sharedMaterial == null)
            {
                skipped++;
                continue;
            }

            if (meshRenderer.gameObject == root)
            {
                continue;
            }

            Mesh mesh = filter.sharedMesh;
            Material material = meshRenderer.sharedMaterial;
            material.enableInstancing = true;
            EditorUtility.SetDirty(material);

            long key = ((long)mesh.GetInstanceID() << 32) ^ (uint)material.GetInstanceID();
            if (!groups.TryGetValue(key, out List<Matrix4x4> matrices))
            {
                matrices = new List<Matrix4x4>(256);
                groups.Add(key, matrices);
                meshes.Add(key, mesh);
                materials.Add(key, material);
            }

            matrices.Add(rootInverse * meshRenderer.transform.localToWorldMatrix);
        }

        if (groups.Count == 0)
        {
            // Debug.LogError("L2InstancedPropBaker: no mesh renderers under " + root.name);
            return null;
        }

        var batches = new L2InstancedPropBatch[groups.Count];
        int batchIndex = 0;
        int total = 0;
        foreach (var pair in groups)
        {
            batches[batchIndex++] = new L2InstancedPropBatch
            {
                mesh = meshes[pair.Key],
                material = materials[pair.Key],
                matrices = pair.Value.ToArray(),
                castShadows = castShadows
            };
            total += pair.Value.Count;
        }

        L2InstancedPropSet set = AssetDatabase.LoadAssetAtPath<L2InstancedPropSet>(assetPath);
        if (set == null)
        {
            set = ScriptableObject.CreateInstance<L2InstancedPropSet>();
            Directory.CreateDirectory(Path.GetDirectoryName(assetPath) ?? "Assets");
            AssetDatabase.CreateAsset(set, assetPath);
        }

        set.batches = batches;
        set.instanceCount = total;
        EditorUtility.SetDirty(set);
        AssetDatabase.SaveAssets();
        // Debug.Log(
            // "L2InstancedPropBaker: baked " +
            // total +
            // " instances / " +
            // batches.Length +
            // " batches → " +
            // assetPath +
            // (skipped > 0 ? " (skipped " + skipped + ")" : string.Empty));
        return set;
    }
}
#endif
