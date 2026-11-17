#if UNITY_EDITOR
// Compare every mirror convention of the original bind pose on both hands, with
// the body in its idle pose, from the front and from the side.
using System.IO;
using UnityEditor;
using UnityEngine;

public static class L2FistBindVariants
{
    const string Anim = "Assets/Resources/Data/Animations/";
    const string OutDir = "/tmp/fineage-fist-variants";

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
        var pfW = AssetDatabase.LoadAssetAtPath<GameObject>(Anim + "LineageWeapons/demon_splinter_m00_wp.prefab");

        Quaternion q = new Quaternion(-0.1075f, 0.2438f, 0.1606f, 0.9504f);
        Vector3 p = new Vector3(15.170f, -1.284f, -4.038f);

        // Candidates for converting a UE2 transform into the mirrored FBX space.
        var candidates = new (string label, Quaternion rot, Vector3 pos)[]
        {
            ("raw", q, p),
            ("flipX", new Quaternion(q.x, -q.y, -q.z, q.w), new Vector3(-p.x, p.y, p.z)),
            ("flipY", new Quaternion(-q.x, q.y, -q.z, q.w), new Vector3(p.x, -p.y, p.z)),
            ("flipZ", new Quaternion(-q.x, -q.y, q.z, q.w), new Vector3(p.x, p.y, -p.z)),
            ("negQuat", new Quaternion(-q.x, -q.y, -q.z, q.w), new Vector3(-p.x, -p.y, -p.z)),
            ("rawRot_mirrorPosX", q, new Vector3(-p.x, p.y, p.z)),
            ("rawRot_noPos", q, Vector3.zero),
        };

        foreach (var (label, rot, pos) in candidates)
        {
            var r = (GameObject)PrefabUtility.InstantiatePrefab(pfW);
            r.transform.SetParent(wr, false);
            Vector3 lossy = r.transform.lossyScale;
            r.transform.localScale = new Vector3(1f / lossy.x, 1f / lossy.y, 1f / lossy.z);
            r.transform.localRotation = rot;
            r.transform.localPosition = pos * 0.019f;

            Shoot(hand.position, label + "_side", new Vector3(0.30f, 0.02f, -0.42f), 30f);
            Shoot(hand.position, label + "_front", new Vector3(0.02f, 0.05f, -0.50f), 30f);
            Object.DestroyImmediate(r);
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