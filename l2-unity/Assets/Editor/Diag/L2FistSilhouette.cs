#if UNITY_EDITOR
// Unlit silhouette sheet: the body in flat grey, the weapon in flat red. Lit renders of
// thin claws came out as a dark smear; flat colours show the actual shape and direction.
//
// Per weapon: raw mount on the left, per-weapon correction on the right.
// Side view is from the character's right; front view looks the character in the face.
using System.IO;
using UnityEditor;
using UnityEngine;

public static class L2FistSilhouette
{
    const string Anim = "Assets/Resources/Data/Animations/";
    const string OutDir = "/tmp/fineage-fist-silhouette";

    struct Weapon
    {
        public string model;
        public Quaternion fix;
        public bool control;
    }

    static readonly Weapon[] Weapons =
    {
        new Weapon { model = "fist_blade_m00_wp",        fix = new Quaternion( 0.0000f,  0.0000f, -0.0000f,  1.0000f) },
        new Weapon { model = "the_fist_of_hero_m00_wp",  fix = new Quaternion(-0.0000f, -0.0000f, -0.7071f,  0.7071f) },
        new Weapon { model = "great_pata_m00_wp",        fix = new Quaternion( 0.7230f, -0.2995f,  0.2382f, -0.5751f) },
        new Weapon { model = "bellion_cestus_m00_wp",    fix = new Quaternion( 0.5133f, -0.2126f, -0.3182f,  0.7682f) },
        new Weapon { model = "demon_splinter_m00_wp",    fix = new Quaternion( 0.5732f, -0.1737f, -0.0220f,  0.8005f) },
        new Weapon { model = "demon_fangs_m00_wp",       fix = new Quaternion( 0.7011f, -0.0923f,  0.5610f,  0.4305f) },
        new Weapon { model = "arthro_nail_m00_wp",       fix = new Quaternion( 0.6533f, -0.2706f, -0.6533f, -0.2706f) },
        new Weapon { model = "foxs_nail_m00_wp",         fix = new Quaternion( 0.6533f, -0.2706f, -0.6533f, -0.2706f) },
        new Weapon { model = "chakram_m00_wp",           fix = new Quaternion( 0.6533f,  0.2706f,  0.6533f, -0.2706f) },
        new Weapon { model = "dragon_grinder_m00_wp",    fix = new Quaternion( 0.6533f, -0.2706f,  0.2706f, -0.6533f) },
        new Weapon { model = "sword_of_damascus_m00_wp", fix = new Quaternion( 0.0000f,  0.0000f,  0.0000f,  1.0000f), control = true },
    };

    static Material _grey;
    static Material _red;

    public static void RunCli()
    {
        Directory.CreateDirectory(OutDir);
        var unlit = Shader.Find("Unlit/Color");
        if (unlit == null) { Debug.LogError("Unlit/Color missing"); EditorApplication.Exit(1); return; }
        _grey = new Material(unlit) { color = new Color(0.72f, 0.74f, 0.78f, 1f) };
        _red = new Material(unlit) { color = new Color(0.95f, 0.15f, 0.15f, 1f) };

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
        Paint(rig, _grey);

        Transform wr = Find(rig, "Weapon_R_Bone");
        Transform wl = Find(rig, "Weapon_L_Bone");
        Transform hand = Find(rig, "Bip01_R_Hand");
        Vector3 focus = hand.position;

        for (int i = 0; i < Weapons.Length; i++)
        {
            Weapon w = Weapons[i];
            var pf = AssetDatabase.LoadAssetAtPath<GameObject>(Anim + "LineageWeapons/" + w.model + ".prefab");
            if (pf == null) { Debug.LogWarning("missing " + w.model); continue; }
            string stem = i.ToString("00") + "_" + w.model;

            Clear(wr); Clear(wl);
            var raw = Mount(pf, wr, null, _red);
            Side(stem + "_raw", focus);
            Object.DestroyImmediate(raw);

            Clear(wr); Clear(wl);
            var fix = Mount(pf, wr, w.fix, _red);
            Side(stem + "_fix", focus);
            Object.DestroyImmediate(fix);

            Clear(wr); Clear(wl);
        }

        Object.DestroyImmediate(_grey);
        Object.DestroyImmediate(_red);
        EditorApplication.Exit(0);
    }

    static GameObject Mount(GameObject prefab, Transform socket, Quaternion? rot, Material mat)
    {
        var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        go.transform.SetParent(socket, false);
        Vector3 lossy = go.transform.lossyScale;
        float x = Mathf.Abs(1f / lossy.x);
        go.transform.localScale = new Vector3(x, 1f / lossy.y, 1f / lossy.z);
        if (rot.HasValue) go.transform.localRotation = rot.Value;
        Paint(go, mat);
        return go;
    }

    static void Paint(GameObject root, Material mat)
    {
        foreach (var r in root.GetComponentsInChildren<Renderer>(true))
        {
            var mats = new Material[r.sharedMaterials.Length];
            for (int i = 0; i < mats.Length; i++) mats[i] = mat;
            if (mats.Length == 0) mats = new[] { mat };
            r.sharedMaterials = mats;
            r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
    }

    static void Clear(Transform socket)
    {
        for (int i = socket.childCount - 1; i >= 0; i--)
            Object.DestroyImmediate(socket.GetChild(i).gameObject);
    }

    static void Side(string label, Vector3 focus)
    {
        // framed on the hand plus the whole forearm so mount angle is readable
        Shoot(focus + new Vector3(0.26f, 0.05f, -0.40f), label, focus, 40f);
    }

    static Transform Find(GameObject root, string name)
    {
        foreach (var t in root.GetComponentsInChildren<Transform>(true))
            if (t.name == name) return t;
        return null;
    }

    static void Shoot(Vector3 camPos, string label, Vector3 focus, float fov)
    {
        var camGo = new GameObject("cam");
        var cam = camGo.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.10f, 0.10f, 0.13f, 1f);
        cam.fieldOfView = fov;
        cam.nearClipPlane = 0.005f;
        cam.farClipPlane = 50f;
        cam.transform.position = camPos;
        cam.transform.LookAt(focus);

        const int size = 800;
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
    }
}
#endif
