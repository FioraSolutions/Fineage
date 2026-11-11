#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static partial class L2InstancedPropBaker
{
    [MenuItem("L2/Map/Bake l2_lobby DecoLayer To Instances")]
    public static void BakeLobbyDeco()
    {
        BakeDecoPrefab(
            "Assets/Resources/Data/Maps/l2_lobby/DecoLayer.prefab",
            "Assets/Resources/Data/Maps/l2_lobby/l2_lobby_DecoInstances.asset");
    }

    public static void BakeDecoPrefab(
        string prefabPath,
        string setPath,
        float maxDrawDistance = 30f,
        int layer = -1)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null)
        {
            // Debug.LogError("L2InstancedPropBaker: missing " + prefabPath);
            return;
        }

        if (layer < 0)
        {
            layer = LayerMask.NameToLayer("Deco");
        }

        GameObject instance = Object.Instantiate(prefab);
        instance.name = prefab.name;
        L2InstancedPropSet set = ReplaceHierarchyWithInstances(
            instance,
            setPath,
            castShadows: false,
            maxDrawDistance: maxDrawDistance,
            layer: layer);

        if (set == null)
        {
            Object.DestroyImmediate(instance);
            return;
        }

        PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
        Object.DestroyImmediate(instance);
        AssetDatabase.SaveAssets();
        // Debug.Log(
            // "L2InstancedPropBaker: " +
            // prefabPath +
            // " is now instanced (" +
            // set.instanceCount +
            // " props, " +
            // set.batches.Length +
            // " batches).");
    }
}
#endif
