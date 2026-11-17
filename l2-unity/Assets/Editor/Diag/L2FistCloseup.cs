#if UNITY_EDITOR
// Large close-ups of the right fist, from two angles, for every candidate mount model.
// The claws are thin and the hand is small, so the earlier 420px contact sheet was not
// enough to judge "points forward out of the knuckles" vs "lies across the forearm".
using System.IO;
using UnityEditor;
using UnityEngine;

public static class L2FistCloseup
{
    const string Anim = "Assets/Resources/Data/Animations/";
    const string OutDir = "/tmp/fineage-fist-closeup";

    struct Weapon
    {
        public string model;
        public Quaternion bindRot;
    }

    static readonly Weapon[] Weapons =
    {
        new Weapon { model = "fist_blade_m00_wp",     bindRot = new Quaternion(-0.653281f,  0.270598f,  0.270598f, 0.653282f) },
        new Weapon { model = "demon_splinter_m00_wp", bindRot = new Quaternion(-0.107474f,  0.243822f,  0.160617f, 0.950370f) },
        new Weapon { model = "great_pata_m00_wp",     bindRot = new Quaternion( 0.993572f,  0.000000f, -0.000000f, 0.113203f) },
        new Weapon { model = "sword_of_damascus_m00_wp", bindRot = new Quaternion( 0.707107f, -0.707107f, 0.000000f, 0.000000f) },
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
        Vector3 focus = hand.position;

        for (int i = 0; i < Weapons.Length; i++)
        {
            Weapon w = Weapons[i];
            var pf = AssetDatabase.LoadAssetAtPath<GameObject>(Anim + "LineageWeapons/" + w.model + ".prefab");
            if (pf == null) continue;
            string stem = i.ToString("00") + "_" + w.model;

            Mount(pf, wr, null);
            Two(stem + "_a_identity", focus);

            Mount(pf, wr, w.bindRot);
            Two(stem + "_b_bind", focus);

            Mount(pf, wr, Quaternion.Inverse(w.bindRot));
            Two(stem + "_c_invbind", focus);

            Clear(wr);
        }

        EditorApplication.Exit(0);
    }

    static void Mount(GameObject prefab, Transform socket, Quaternion? rot)
    {
        Clear(socket);
        var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        go.transform.SetParent(socket, false);
        Vector3 lossy = go.transform.lossyScale;
        go.transform.localScale = new Vector3(1f / lossy.x, 1f / lossy.y, 1f / lossy.z);
        if (rot.HasValue) go.transform.localRotation = rot.Value;
    }

    static void Clear(Transform socket)
    {
        for (int i = socket.childCount - 1; i >= 0; i--)
            Object.DestroyImmediate(socket.GetChild(i).gameObject);
    }

    static void Two(string stem, Vector3 focus)
    {
        // outboard side view: the arm comes from the left, the hand is at the right
        Shoot(focus + new Vector3(0.16f, 0.02f, -0.20f), stem + "_side", focus);
        // front view: looks the character in the face
        Shoot(focus + new Vector3(0.03f, 0.03f, -0.26f), stem + "_front", focus);
    }

    static Transform Find(GameObject root, string name)
    {
        foreach (var t in root.GetComponentsInChildren<Transform>(true))
            if (t.name == name) return t;
        return null;
    }

    static void Shoot(Vector3 camPos, string label, Vector3 focus)
    {
        var camGo = new GameObject("cam");
        var cam = camGo.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.16f, 0.16f, 0.19f, 1f);
        cam.fieldOfView = 24f;
        cam.nearClipPlane = 0.005f;
        cam.farClipPlane = 50f;
        cam.transform.position = camPos;
        cam.transform.LookAt(focus);

        var lightGo = new GameObject("light");
        var light = lightGo.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.8f;
        lightGo.transform.rotation = Quaternion.Euler(34f, 140f, 0f);

        var fillGo = new GameObject("fill");
        var fill = fillGo.AddComponent<Light>();
        fill.type = LightType.Directional;
        fill.intensity = 0.7f;
        fillGo.transform.rotation = Quaternion.Euler(10f, -60f, 0f);

        const int size = 900;
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
        Object.DestroyImmediate(fillGo);
    }
}
#endif
