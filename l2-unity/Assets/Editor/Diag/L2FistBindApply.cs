#if UNITY_EDITOR
// Apply the original PSK bind pose (single bone) to the fist weapon and photograph
// the result. Raw FBX vertices are (-x, y, z) * 0.019 of the UE2 points, so the
// UE2 bind transform must be mirrored the same way.
using System.IO;
using UnityEditor;
using UnityEngine;

public static class L2FistBindApply
{
    const string Anim = "Assets/Resources/Data/Animations/";
    const string OutDir = "/tmp/fineage-fist-bind";

    public static void RunCli()
    {
        Directory.CreateDirectory(OutDir);

        // PSK REFSKELT for demon_splinter_m00_wp (UE2 space).
        Quaternion ue2 = new Quaternion(-0.1075f, 0.2438f, 0.1606f, 0.9504f);
        Vector3 ue2Pos = new Vector3(15.170f, -1.284f, -4.038f);

        // Mirror X on the quaternion: for M = diag(-1,1,1), M*R*M flips the
        // rotation axis components that lie in the mirror plane.
        Quaternion bind = new Quaternion(ue2.x, -ue2.y, -ue2.z, ue2.w);
        Vector3 bindPos = new Vector3(-ue2Pos.x, ue2Pos.y, ue2Pos.z) * 0.019f;

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

        // variant A: only rotation
        var a = (GameObject)PrefabUtility.InstantiatePrefab(pfW);
        a.transform.SetParent(wr, false);
        Vector3 lossy = a.transform.lossyScale;
        a.transform.localScale = new Vector3(1f / lossy.x, 1f / lossy.y, 1f / lossy.z);
        a.transform.localRotation = bind;
        Shoot(hand.position, "bind_rot_only", new Vector3(0.30f, 0.02f, -0.42f), 30f);
        Object.DestroyImmediate(a);

        // variant B: rotation and translation
        var b = (GameObject)PrefabUtility.InstantiatePrefab(pfW);
        b.transform.SetParent(wr, false);
        lossy = b.transform.lossyScale;
        b.transform.localScale = new Vector3(1f / lossy.x, 1f / lossy.y, 1f / lossy.z);
        b.transform.localRotation = bind;
        b.transform.localPosition = bindPos;
        Shoot(hand.position, "bind_rot_pos", new Vector3(0.30f, 0.02f, -0.42f), 30f);
        Object.DestroyImmediate(b);

        // variant C: no mirroring of the quaternion
        var c = (GameObject)PrefabUtility.InstantiatePrefab(pfW);
        c.transform.SetParent(wr, false);
        lossy = c.transform.lossyScale;
        c.transform.localScale = new Vector3(1f / lossy.x, 1f / lossy.y, 1f / lossy.z);
        c.transform.localRotation = ue2;
        Shoot(hand.position, "bind_ue2_quat", new Vector3(0.30f, 0.02f, -0.42f), 30f);
        Object.DestroyImmediate(c);

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