#if UNITY_EDITOR
// Photograph each fist model on the same human hand from the same camera, so the
// difference in how the mesh sits is visible rather than inferred.
using System.IO;
using UnityEditor;
using UnityEngine;

public static class L2FistModelCompare
{
    const string Anim = "Assets/Resources/Data/Animations/";
    const string OutDir = "/tmp/fineage-fist-models";

    static readonly string[] Models =
    {
        "fist_blade_m00_wp",
        "demon_splinter_m00_wp",
        "great_pata_m00_wp",
        "demon_fangs_m00_wp",
        "the_fist_of_hero_m00_wp",
    };

    public static void RunCli()
    {
        Directory.CreateDirectory(OutDir);

        var rigPf = AssetDatabase.LoadAssetAtPath<GameObject>(Anim + "Fighter/MFighter/MFighter_anim_player.prefab");
        var rig = (GameObject)PrefabUtility.InstantiatePrefab(rigPf);

        foreach (string part in new[] { "u", "l", "g", "b" })
        {
            var pf = AssetDatabase.LoadAssetAtPath<GameObject>(Anim + "Fighter/MFighter_m000_" + part + ".prefab");
            if (pf == null) continue;
            var piece = (GameObject)PrefabUtility.InstantiatePrefab(pf);
            piece.transform.SetParent(rig.transform.parent != null ? rig.transform.parent : rig.transform, false);
        }
        rig.AddComponent<SkinnedMeshSync>().SyncMesh();

        Transform wr = Find(rig, "Weapon_R_Bone");
        Transform hand = Find(rig, "Bip01_R_Hand");

        foreach (string name in Models)
        {
            var pf = AssetDatabase.LoadAssetAtPath<GameObject>(Anim + "LineageWeapons/" + name + ".prefab");
            if (pf == null) continue;
            var w = (GameObject)PrefabUtility.InstantiatePrefab(pf);
            w.transform.SetParent(wr, false);
            Vector3 lossy = w.transform.lossyScale;
            w.transform.localScale = new Vector3(1f / lossy.x, 1f / lossy.y, 1f / lossy.z);

            Shoot(hand.position, name + "_side", new Vector3(0.30f, 0.02f, -0.42f), 30f);
            Shoot(hand.position, name + "_front", new Vector3(0.02f, 0.05f, -0.50f), 30f);
            Object.DestroyImmediate(w);
        }

        EditorApplication.Exit(0);
    }

    static Transform Find(GameObject root, string name)
    {
        foreach (var t in root.GetComponentsInChildren<Transform>(true))
            if (t.name == name) return t;
        return null;
    }

    static void Shoot(Vector3 focus, string label, Vector3 offset, float fov)
    {
        var camGo = new GameObject("cam");
        var cam = camGo.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.12f, 0.12f, 0.14f, 1f);
        cam.fieldOfView = fov;
        cam.nearClipPlane = 0.005f;
        cam.farClipPlane = 50f;
        cam.transform.position = focus + offset;
        cam.transform.LookAt(focus);

        var lightGo = new GameObject("light");
        var light = lightGo.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.5f;
        lightGo.transform.rotation = Quaternion.Euler(25f, 150f, 0f);

        const int size = 560;
        var rt = new RenderTexture(size, size, 24, RenderTextureFormat.ARGB32);
        cam.targetTexture = rt;
        cam.Render();
        RenderTexture.active = rt;
        var tex = new Texture2D(size, size, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, size, size), 0, 0);
        tex.Apply();
        RenderTexture.active = null;
        File.WriteAllBytes(Path.Combine(OutDir, label + ".png"), tex.EncodeToPNG());
        Object.DestroyImmediate(tex);
        rt.Release();
        Object.DestroyImmediate(rt);
        Object.DestroyImmediate(camGo);
        Object.DestroyImmediate(lightGo);
    }
}
#endif