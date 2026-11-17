#if UNITY_EDITOR
// Try rotating the S-grade fist around its long axis and see whether the claw
// ends up in front of the fist like the original reference instead of lying on
// the back of the hand.
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class L2FistRotationTest
{
    const string Anim = "Assets/Resources/Data/Animations/";
    const string OutDir = "/tmp/fineage-fist-rot";

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
        var pfW = AssetDatabase.LoadAssetAtPath<GameObject>(Anim + "LineageWeapons/demon_splinter_m00_wp.prefab");

        foreach (string spec in new[] { "base:0,0,0", "x90:90,0,0", "xm90:-90,0,0", "z90:0,0,90", "zm90:0,0,-90", "x90z90:90,0,90" })
        {
            string[] parts = spec.Split(':');
            string label = parts[0];
            float[] euler = parts[1].Split(',').Select(float.Parse).ToArray();
            var w = (GameObject)PrefabUtility.InstantiatePrefab(pfW);
            w.transform.SetParent(wr, false);
            Vector3 lossy = w.transform.lossyScale;
            w.transform.localScale = new Vector3(1f / lossy.x, 1f / lossy.y, 1f / lossy.z);
            w.transform.localRotation = Quaternion.Euler(euler[0], euler[1], euler[2]);

            Shoot(hand.position, label, new Vector3(0.30f, 0.02f, -0.42f), 30f);
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