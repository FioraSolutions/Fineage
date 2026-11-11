using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SkinnedMeshSync : MonoBehaviour
{
    static Dictionary<string, Transform> BuildSkeletonMap(Transform animRoot)
    {
        var map = new Dictionary<string, Transform>(128);
        Transform[] all = animRoot.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < all.Length; i++)
        {
            Transform t = all[i];
            if (t == null || string.IsNullOrEmpty(t.name))
                continue;

            // Exact name first; lowercase alias for Interlude FBX case drift (bip01_spine vs Bip01_Spine).
            if (!map.ContainsKey(t.name))
                map[t.name] = t;

            string lower = t.name.ToLowerInvariant();
            if (!map.ContainsKey(lower))
                map[lower] = t;
        }
        return map;
    }








}
