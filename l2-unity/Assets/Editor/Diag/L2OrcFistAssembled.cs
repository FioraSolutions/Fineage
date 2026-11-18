#if UNITY_EDITOR
// Assemble an orc fighter exactly the way the game does (anim rig + gear pieces
// remapped through SkinnedMeshSync), mount the fist weapon through the same
// SetParentGo path, and measure where the claw sits relative to the fist.
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class L2OrcFistAssembled
{
    const string Anim = "Assets/Resources/Data/Animations/";
    const string OutDir = "/tmp/fineage-orc-assembled";

    public static void RunCli()
    {
        Directory.CreateDirectory(OutDir);
        var sb = new StringBuilder();
        var rigPf = AssetDatabase.LoadAssetAtPath<GameObject>(Anim + "Orc/MOrc/MOrc_anim_player.prefab");
        if (rigPf == null) { Debug.LogError("no rig"); EditorApplication.Exit(1); return; }

        var actor = new GameObject("actor");
        var rig = (GameObject)PrefabUtility.InstantiatePrefab(rigPf);
        rig.transform.SetParent(actor.transform, false);

        // gear pieces as siblings of the anim rig, like UserGear's container
        foreach (string part in new[] { "u", "l", "g", "b" })
        {
            var pf = AssetDatabase.LoadAssetAtPath<GameObject>(Anim + "Orc/MOrc_m000_" + part + ".prefab");
            if (pf == null) { sb.AppendLine("missing " + part); continue; }
            var piece = (GameObject)PrefabUtility.InstantiatePrefab(pf);
            piece.transform.SetParent(actor.transform, false);
        }

        // drive the real remap
        var sync = rig.AddComponent<SkinnedMeshSync>();
        var m = typeof(SkinnedMeshSync).GetMethod("SyncMesh", BindingFlags.Public | BindingFlags.Instance);
        if (m != null) m.Invoke(sync, null);

        Transform wr = Find(rig, "Weapon_R_Bone");
        Transform hand = Find(rig, "Bip01_R_Hand");
        sb.AppendLine($"rig Weapon_R_Bone world={V(wr.position)} localPos={V(wr.localPosition)} lossy={V(wr.lossyScale)}");
        sb.AppendLine($"rig Bip01_R_Hand   world={V(hand.position)}");

        // hand vertex cloud in world space after the remap
        foreach (var smr in actor.GetComponentsInChildren<SkinnedMeshRenderer>(true))
        {
            if (smr.sharedMesh == null) continue;
            var mesh = new Mesh();
            smr.BakeMesh(mesh);
            var verts = mesh.vertices;
            if (verts.Length == 0) { Object.DestroyImmediate(mesh); continue; }

            // vertices that belong to the right hand: nearest to hand bone in bind space
            Bounds handBounds = new Bounds(hand.position, Vector3.zero);
            float radius = 0.14f;
            var pts = new List<Vector3>();
            var world = new List<Vector3>(verts.Length);
            for (int i = 0; i < verts.Length; i++)
            {
                Vector3 w = smr.transform.TransformPoint(verts[i]);
                world.Add(w);
                if ((w - hand.position).sqrMagnitude < radius * radius) pts.Add(w);
            }
            if (pts.Count > 0)
            {
                Bounds hb = new Bounds(pts[0], Vector3.zero);
                foreach (var p in pts) hb.Encapsulate(p);
                sb.AppendLine($"  {smr.name}: {pts.Count} hand verts, centre={V(hb.center)} size={V(hb.size)}");
                sb.AppendLine($"     socket offset from hand centre: {V(wr.position - hb.center)} (dist={Vector3.Distance(wr.position, hb.center):F4})");
            }
            Object.DestroyImmediate(mesh);
        }

        // mount the fist weapon exactly like SetParentGo
        var wp = AssetDatabase.LoadAssetAtPath<GameObject>(Anim + "LineageWeapons/fist_blade_m00_wp.prefab");
        var weapon = (GameObject)PrefabUtility.InstantiatePrefab(wp);
        weapon.transform.SetParent(wr, false);
        // NormalizeWeaponWorldScale equivalent
        Vector3 lossy = weapon.transform.lossyScale;
        weapon.transform.localScale = new Vector3(1f / lossy.x, 1f / lossy.y, 1f / lossy.z);

        var mf = weapon.GetComponentInChildren<MeshFilter>();
        if (mf != null)
        {
            Bounds b = mf.sharedMesh.bounds;
            sb.AppendLine($"weapon local bounds centre={V(b.center)} size={V(b.size)}");
            sb.AppendLine($"weapon world scale={V(weapon.transform.lossyScale)}");
            Vector3 wc = weapon.transform.TransformPoint(b.center);
            sb.AppendLine($"weapon world centre={V(wc)}  socket-to-weapon-centre={Vector3.Distance(wc, wr.position):F4}");
        }

        // render a close-up of the right hand with the claw
        Shoot(hand.position, "MOrc_hand", new Vector3(0f, 0.10f, -0.55f), 32f);
        Shoot(actor.transform.position + Vector3.up * 0.9f, "MOrc_body", new Vector3(0.4f, 0.2f, -2.6f), 35f);

        File.WriteAllText(Path.Combine(OutDir, "report.txt"), sb.ToString());
        Debug.Log(sb.ToString());
        EditorApplication.Exit(0);
    }

    static Transform Find(GameObject root, string name)
    {
        foreach (var t in root.GetComponentsInChildren<Transform>(true))
            if (t.name == name) return t;
        return null;
    }

    static string V(Vector3 v) => "(" + v.x.ToString("F4") + "," + v.y.ToString("F4") + "," + v.z.ToString("F4") + ")";

    static void Shoot(Vector3 focus, string label, Vector3 offset, float fov)
    {
        var camGo = new GameObject("cam");
        var cam = camGo.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.12f, 0.12f, 0.14f, 1f);
        cam.fieldOfView = fov;
        cam.nearClipPlane = 0.01f;
        cam.farClipPlane = 50f;
        cam.transform.position = focus + offset;
        cam.transform.LookAt(focus);

        var lightGo = new GameObject("light");
        var light = lightGo.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.4f;
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