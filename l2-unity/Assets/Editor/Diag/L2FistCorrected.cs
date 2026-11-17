#if UNITY_EDITOR
// Contact sheet for the per-weapon fist correction.
//
// The mount convention is identity: swords sit right with raw PSK vertices, and their bind
// poses (98 deg, 148 deg, 180 deg) are as varied as the fists', so the game does not apply
// the bind pose. What differs between fists is the mesh itself: each .psk authors its claws
// along a different local axis (-Y for fist_blade, +X for demon_fangs, and so on).
//
// fist_blade is the anchor because the user reports it sits right today. A weapon's
// correction is therefore the rotation that carries its own bind frame onto fist_blade's:
//     correction = inverse(bind_anchor) * bind_weapon
// which is exactly the per-weapon axis difference, with the anchor left untouched.
//
// Each row is one weapon: raw on the left, corrected on the right, from a side view and
// from the front, at 900px so a 3 cm claw is actually legible.
using System.IO;
using UnityEditor;
using UnityEngine;

public static class L2FistCorrected
{
    const string Anim = "Assets/Resources/Data/Animations/";
    const string OutDir = "/tmp/fineage-fist-corrected";

    struct Weapon
    {
        public string model;
        public Quaternion fix;
    }

    static readonly Weapon[] Weapons =
    {
        // anchor: already correct, correction is identity by construction
        new Weapon { model = "fist_blade_m00_wp",       fix = new Quaternion( 0.0000f,  0.0000f, -0.0000f, 1.0000f) },
        new Weapon { model = "the_fist_of_hero_m00_wp", fix = new Quaternion(-0.0000f, -0.0000f, -0.7071f, 0.7071f) },
        new Weapon { model = "great_pata_m00_wp",       fix = new Quaternion( 0.7230f, -0.2995f,  0.2382f, -0.5751f) },
        new Weapon { model = "bellion_cestus_m00_wp",   fix = new Quaternion( 0.5133f, -0.2126f, -0.3182f, 0.7682f) },
        new Weapon { model = "demon_splinter_m00_wp",   fix = new Quaternion( 0.5732f, -0.1737f, -0.0220f, 0.8005f) },
        new Weapon { model = "demon_fangs_m00_wp",      fix = new Quaternion( 0.7011f, -0.0923f,  0.5610f, 0.4305f) },
        new Weapon { model = "arthro_nail_m00_wp",      fix = new Quaternion( 0.6533f, -0.2706f, -0.6533f, -0.2706f) },
        new Weapon { model = "foxs_nail_m00_wp",        fix = new Quaternion( 0.6533f, -0.2706f, -0.6533f, -0.2706f) },
        new Weapon { model = "chakram_m00_wp",          fix = new Quaternion( 0.6533f,  0.2706f,  0.6533f, -0.2706f) },
        new Weapon { model = "dragon_grinder_m00_wp",   fix = new Quaternion( 0.6533f, -0.2706f,  0.2706f, -0.6533f) },
        // control: a 1H sword, whose correction must stay identity or the control is useless
        new Weapon { model = "sword_of_damascus_m00_wp", fix = new Quaternion( 0.0000f, 0.0000f, 0.0000f, 1.0000f) },
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
        Transform wl = Find(rig, "Weapon_L_Bone");
        Transform hand = Find(rig, "Bip01_R_Hand");
        Transform handL = Find(rig, "Bip01_L_Hand");
        Vector3 focusR = hand.position;
        Vector3 focusL = handL != null ? handL.position : focusR;

        for (int i = 0; i < Weapons.Length; i++)
        {
            Weapon w = Weapons[i];
            var pf = AssetDatabase.LoadAssetAtPath<GameObject>(Anim + "LineageWeapons/" + w.model + ".prefab");
            if (pf == null) { Debug.LogWarning("missing " + w.model); continue; }
            string stem = i.ToString("00") + "_" + w.model;

            Mount(pf, wr, null, false);
            Side(stem + "_raw", focusR);

            Mount(pf, wr, w.fix, false);
            Side(stem + "_fix", focusR);

            // both hands at once with the correction: the left copy is mirrored, so this is
            // the only way to see that the correction survives the negative X scale
            Clear(wr); Clear(wl);
            Mount2(pf, wr, w.fix, false);
            Mount2(pf, wl, w.fix, true);
            Side(stem + "_both", focusR);
            Clear(wr); Clear(wl);
        }

        EditorApplication.Exit(0);
    }

    static void Mount(GameObject prefab, Transform socket, Quaternion? rot, bool mirror)
    {
        Clear(socket);
        Mount2(prefab, socket, rot, mirror);
    }

    static void Mount2(GameObject prefab, Transform socket, Quaternion? rot, bool mirror)
    {
        var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        go.transform.SetParent(socket, false);
        Vector3 lossy = go.transform.lossyScale;
        float x = Mathf.Abs(1f / lossy.x);
        go.transform.localScale = new Vector3(mirror ? -x : x, 1f / lossy.y, 1f / lossy.z);
        if (rot.HasValue) go.transform.localRotation = rot.Value;
    }

    static void Clear(Transform socket)
    {
        for (int i = socket.childCount - 1; i >= 0; i--)
            Object.DestroyImmediate(socket.GetChild(i).gameObject);
    }

    static void Side(string label, Vector3 focus)
    {
        // far enough back that the forearm, the fist and the whole claw fit the frame:
        // framed on the hand alone the claws run off the edge and cannot be judged
        Shoot(focus + new Vector3(0.30f, 0.06f, -0.52f), label, focus, 34f);
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
        cam.backgroundColor = new Color(0.16f, 0.16f, 0.19f, 1f);
        cam.fieldOfView = fov;
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
