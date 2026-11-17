#if UNITY_EDITOR
// Contact sheet: every fist weapon on the right hand of one character, rendered twice --
// once as the FBX ships it (raw PSK vertices, identity local) and once with the authored
// PSK bind pose applied. Two swords are included as a control: the user reports swords sit
// right with the raw mount, so whichever variant keeps the swords right is the same variant
// that must be applied to the fists.
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class L2FistGrid
{
    const string Anim = "Assets/Resources/Data/Animations/";
    const string OutDir = "/tmp/fineage-fist-grid";

    struct Weapon
    {
        public string model;
        public Quaternion bindRot;
        public Vector3 bindPos;
        public bool control;
    }

    // Threaded from the original .psk REFSKELT chunks (single root bone):
    // name[64], flags/parent/children[12], quat[16], position[12], length/scale.
    static readonly Weapon[] Weapons =
    {
        new Weapon { model = "fist_blade_m00_wp",      bindRot = new Quaternion(-0.653281f,  0.270598f,  0.270598f, 0.653282f), bindPos = new Vector3( -3.8108f, -0.7197f, -0.0168f) },
        new Weapon { model = "the_fist_of_hero_m00_wp", bindRot = new Quaternion(-0.653281f, -0.270598f, -0.270598f, 0.653281f), bindPos = new Vector3( -1.2588f,  6.8440f,  1.4054f) },
        new Weapon { model = "great_pata_m00_wp",       bindRot = new Quaternion( 0.993572f,  0.000000f, -0.000000f, 0.113203f), bindPos = new Vector3( -0.0130f, -0.5358f,  1.9198f) },
        new Weapon { model = "bellion_cestus_m00_wp",   bindRot = new Quaternion(-0.195090f,  0.000000f,  0.000000f, 0.980785f), bindPos = new Vector3( -0.0042f, -0.3011f,  1.9745f) },
        new Weapon { model = "demon_splinter_m00_wp",   bindRot = new Quaternion(-0.107474f,  0.243822f,  0.160617f, 0.950370f), bindPos = new Vector3( 15.1705f, -1.2838f, -4.0376f) },
        new Weapon { model = "demon_fangs_m00_wp",      bindRot = new Quaternion( 0.353553f,  0.612372f,  0.353553f, 0.612372f), bindPos = new Vector3( 18.6110f, -0.0042f, -0.0052f) },
        new Weapon { model = "arthro_nail_m00_wp",      bindRot = new Quaternion( 0.500000f, -0.500000f, -0.500000f, 0.500000f), bindPos = new Vector3( -4.8344f, -4.3130f,  1.0601f) },
        new Weapon { model = "foxs_nail_m00_wp",        bindRot = new Quaternion( 0.500000f, -0.500000f, -0.500000f, 0.500000f), bindPos = new Vector3( -6.3558f,  0.6583f,  0.3984f) },
        new Weapon { model = "chakram_m00_wp",          bindRot = new Quaternion( 0.707107f,  0.707107f,  0.000000f, 0.000000f), bindPos = new Vector3( -0.0118f, -4.2048f,  0.0023f) },
        new Weapon { model = "dragon_grinder_m00_wp",   bindRot = new Quaternion( 1.000000f,  0.000000f,  0.000000f, 0.000000f), bindPos = new Vector3(  2.8105f,  0.0263f, -5.5381f) },
        // control: a 1H sword the user reports as sitting right today
        new Weapon { model = "sword_of_damascus_m00_wp", bindRot = new Quaternion( 0.707107f, -0.707107f, 0.000000f, 0.000000f), bindPos = new Vector3( -5.6050f, -0.0270f, -2.7540f), control = true },
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

        for (int i = 0; i < Weapons.Length; i++)
        {
            Weapon w = Weapons[i];
            var pf = AssetDatabase.LoadAssetAtPath<GameObject>(Anim + "LineageWeapons/" + w.model + ".prefab");
            if (pf == null) { Debug.LogWarning("missing " + w.model); continue; }

            // raw: exactly what the game does today
            var raw = (GameObject)PrefabUtility.InstantiatePrefab(pf);
            raw.transform.SetParent(wr, false);
            Normalise(raw);
            Shoot(hand.position, i.ToString("00") + "_" + w.model + "_raw", new Vector3(0.34f, 0.03f, -0.40f), 26f);
            Object.DestroyImmediate(raw);

            // bind: the authored PSK root bone applied on top of the same socket
            if (!w.control)
            {
                var bind = (GameObject)PrefabUtility.InstantiatePrefab(pf);
                bind.transform.SetParent(wr, false);
                Normalise(bind);
                bind.transform.localRotation = w.bindRot;
                bind.transform.localPosition = w.bindPos * 0.019f;
                Shoot(hand.position, i.ToString("00") + "_" + w.model + "_bind", new Vector3(0.34f, 0.03f, -0.40f), 26f);
                Object.DestroyImmediate(bind);
            }
        }

        EditorApplication.Exit(0);
    }

    static void Normalise(GameObject go)
    {
        Vector3 lossy = go.transform.lossyScale;
        go.transform.localScale = new Vector3(1f / lossy.x, 1f / lossy.y, 1f / lossy.z);
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
        cam.backgroundColor = new Color(0.13f, 0.13f, 0.16f, 1f);
        cam.fieldOfView = fov;
        cam.nearClipPlane = 0.005f;
        cam.farClipPlane = 50f;
        cam.transform.position = focus + offset;
        cam.transform.LookAt(focus);

        var lightGo = new GameObject("light");
        var light = lightGo.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.6f;
        lightGo.transform.rotation = Quaternion.Euler(28f, 155f, 0f);

        const int size = 420;
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
