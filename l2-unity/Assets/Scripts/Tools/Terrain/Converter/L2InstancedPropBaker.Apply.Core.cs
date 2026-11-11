#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static partial class L2InstancedPropBaker
{
    public static L2InstancedPropSet ReplaceHierarchyWithInstances(
        GameObject root,
        string assetPath,
        bool castShadows,
        float maxDrawDistance,
        int layer)
    {
        L2InstancedPropSet set = BakeFromRoot(root, assetPath, castShadows);
        if (set == null)
        {
            return null;
        }

        Transform rootTransform = root.transform;
        KeepRuntimeOnlyChildren(rootTransform);
        for (int i = rootTransform.childCount - 1; i >= 0; i--)
        {
            Transform child = rootTransform.GetChild(i);
            if (child.name == "KeptLights")
            {
                continue;
            }

            Object.DestroyImmediate(child.gameObject);
        }

        MeshFilter[] leftoverFilters = root.GetComponents<MeshFilter>();
        for (int i = 0; i < leftoverFilters.Length; i++)
        {
            Object.DestroyImmediate(leftoverFilters[i]);
        }

        MeshRenderer[] leftoverRenderers = root.GetComponents<MeshRenderer>();
        for (int i = 0; i < leftoverRenderers.Length; i++)
        {
            Object.DestroyImmediate(leftoverRenderers[i]);
        }

        L2InstancedPropRenderer renderer = root.GetComponent<L2InstancedPropRenderer>();
        if (renderer == null)
        {
            renderer = root.AddComponent<L2InstancedPropRenderer>();
        }

        renderer.PropSet = set;
        renderer.MaxDrawDistance = maxDrawDistance;
        renderer.Layer = layer >= 0 ? layer : 0;
        return set;
    }

    private static void KeepRuntimeOnlyChildren(Transform root)
    {
        Light[] lights = root.GetComponentsInChildren<Light>(true);
        AudioSource[] audio = root.GetComponentsInChildren<AudioSource>(true);
        ParticleSystem[] particles = root.GetComponentsInChildren<ParticleSystem>(true);
        if (lights.Length == 0 && audio.Length == 0 && particles.Length == 0)
        {
            return;
        }

        Transform keep = root.Find("KeptLights");
        if (keep == null)
        {
            keep = new GameObject("KeptLights").transform;
            keep.SetParent(root, false);
        }

        ReparentKeepers(lights, keep);
        ReparentKeepers(audio, keep);
        ReparentKeepers(particles, keep);
    }
}
#endif
