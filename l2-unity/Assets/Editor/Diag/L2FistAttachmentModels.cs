#if UNITY_EDITOR
// Which transform puts a weapon mesh into the hand?
//
// A Lineage II weapon is a UE2 SkeletalMesh with exactly one bone. The PSK REFSKELT chunk
// carries that bone's bind pose, the FBX export dropped it, and the PSK vertices are raw
// mesh space, so today the weapon is drawn as handWorld * identity * v.
//
// Three candidate attachment models, all rendered on the same socket from the same camera:
//
//   identity : handWorld * v                    (what the game does now)
//   bind     : handWorld * bind * v             (bind pose applied forwards)
//   invBind  : handWorld * inverse(bind) * v    (UE2 AttachToBone: the root bone *is* the
//                                               hand, so the mesh is placed by the inverse)
//
// fist_blade is the discriminator: the user reports it sits right today, so a correct model
// must leave it right AND straighten demon_splinter / great_pata.
using System.IO;
using UnityEditor;
using UnityEngine;

public static class L2FistAttachmentModels
{
    const string Anim = "Assets/Resources/Data/Animations/";
    const string OutDir = "/tmp/fineage-fist-models";

    struct Weapon
    {
        public string model;
        public Quaternion bindRot;
        public Vector3 bindPos;
    }

    // REFSKELT root bone of each PSK: name[64], flags/parent/children[12], quat[16],
    // position[12], length/scale. Quaternion is (x, y, z, w) after the 3 leading zeros.
    static readonly Weapon[] Weapons =
    {
        new Weapon { model = "fist_blade_m00_wp",    bindRot = new Quaternion(-0.653281f,  0.270598f,  0.270598f, 0.653282f), bindPos = new Vector3( -3.8108f, -0.7197f, -0.0168f) },
        new Weapon { model = "demon_splinter_m00_wp", bindRot = new Quaternion(-0.107474f,  0.243822f,  0.160617f, 0.950370f), bindPos = new Vector3( 15.1705f, -1.2838f, -4.0376f) },
        new Weapon { model = "great_pata_m00_wp",    bindRot = new Quaternion( 0.993572f,  0.000000f, -0.000000f, 0.113203f), bindPos = new Vector3( -0.0130f, -0.5358f,  1.9198f) },
        new Weapon { model = "sword_of_damascus_m00_wp", bindRot = new Quaternion( 0.707107f, -0.707107f, 0.000000f, 0.000000f), bindPos = new Vector3( -5.6050f, -0.0270f, -2.7540f) },
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
            if (pf == null) { Debug.LogWarning("missing " + w.model); continue; }

            string stem = i.ToString("00") + "_" + w.model;

            Mount(pf, wr, null, null);
            Shoot(focus, stem + "_a_identity", 30f);

            Mount(pf, wr, w.bindRot, w.bindPos * 0.019f);
            Shoot(focus, stem + "_b_bind", 30f);

            Quaternion invRot = Quaternion.Inverse(w.bindRot);
            Vector3 invPos = -(invRot * (w.bindPos * 0.019f));
            Mount(pf, wr, invRot, invPos);
            Shoot(focus, stem + "_c_invbind", 30f);

            ClearSockets(wr);
        }

        EditorApplication.Exit(0);
    }

    static void Mount(GameObject prefab, Transform socket, Quaternion? rot, Vector3? pos)
    {
        ClearSockets(socket);
        var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        go.transform.SetParent(socket, false);
        Vector3 lossy = go.transform.lossyScale;
        go.transform.localScale = new Vector3(1f / lossy.x, 1f / lossy.y, 1f / lossy.z);
        if (rot.HasValue) go.transform.localRotation = rot.Value;
        if (pos.HasValue) go.transform.localPosition = pos.Value;
    }

    static void ClearSockets(Transform socket)
    {
        for (int i = socket.childCount - 1; i >= 0; i--)
            Object.DestroyImmediate(socket.GetChild(i).gameObject);
    }

    static Transform Find(GameObject root, string name)
    {
        foreach (var t in root.GetComponentsInChildren<Transform>(true))
            if (t.name == name) return t;
        return null;
    }

    static void Shoot(Vector3 focus, string label, float fov)
    {
        var camGo = new GameObject("cam");
        var cam = camGo.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.13f, 0.13f, 0.16f, 1f);
        cam.fieldOfView = fov;
        cam.nearClipPlane = 0.005f;
        cam.farClipPlane = 50f;
        // three-quarter view from the character's right side, so "points forward out of the
        // knuckles" is distinguishable from "lies along the forearm"
        cam.transform.position = focus + new Vector3(0.30f, 0.10f, -0.44f);
        cam.transform.LookAt(focus);

        var lightGo = new GameObject("light");
        var light = lightGo.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.7f;
        lightGo.transform.rotation = Quaternion.Euler(30f, 150f, 0f);

        const int size = 640;
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
