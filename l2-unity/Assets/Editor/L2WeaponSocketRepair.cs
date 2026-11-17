#if UNITY_EDITOR
// Weapon sockets are rigid children of the hand bones. The original client authors their
// offset once, in the body mesh bind, and every race except the orc family still matches
// that offset exactly. On MOrc / FOrc / MShaman the animation rig's Weapon_R_Bone /
// Weapon_L_Bone sit 1.2-2.7 cm away from the body-authored socket, and ~92% of the clips
// carry *constant* position curves that pin the socket to the drifted value every frame.
// A fist weapon is worn on the knuckles, so that drift shows as a floating claw.
//
// Repair, deterministic and idempotent:
//   1. read the socket offset from the race body prefab (the authored bind),
//   2. write it onto the anim rig prefab socket,
//   3. drop the constant socket position/rotation curves that would override it.
// Moving socket curves (bow, chair, death, social) are left untouched.
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class L2WeaponSocketRepair
{
    const string Anim = "Assets/Resources/Data/Animations/";
    const float DriftTolerance = 0.002f;

    static readonly string[] SocketNames = { "Weapon_R_Bone", "Weapon_L_Bone" };

    static readonly (string family, string race)[] Races =
    {
        ("Orc", "MOrc"), ("Orc", "FOrc"), ("Orc", "MShaman"), ("Orc", "FShaman"),
    };

    [MenuItem("Fineage/Repair Weapon Sockets")]
    public static void Run()
    {
        var log = Repair();
        Debug.Log(log);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static void RunCli()
    {
        var log = Repair();
        File.WriteAllText("/tmp/fineage-socket-repair.txt", log);
        Debug.Log(log);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorApplication.Exit(0);
    }

    static string Repair()
    {
        var sb = new StringBuilder();

        foreach (var (family, race) in Races)
        {
            // Shaman bodies live under Shaman/, orc bodies under Orc/. Keep both lookups
            // so a race is never skipped just because its folder family differs.
            string bodyPath = FindFirst(Anim + family + "/" + race + "_m000_u.prefab",
                                        Anim + "Shaman/" + race + "_m000_u.prefab",
                                        Anim + "Orc/" + race + "_m000_u.prefab");
            string rigPath = FindFirst(Anim + family + "/" + race + "/" + race + "_anim_player.prefab",
                                       Anim + "Orc/" + race + "/" + race + "_anim_player.prefab");
            if (bodyPath == null) { sb.AppendLine(race + ": no body"); continue; }
            if (rigPath == null) { sb.AppendLine(race + ": no rig"); continue; }

            var authored = ReadSockets(bodyPath);
            var rigNow = ReadSockets(rigPath);
            sb.AppendLine("=== " + race);

            // 1) + 2) write the authored offset onto the rig prefab
            var rigRoot = PrefabUtility.LoadPrefabContents(rigPath);
            try
            {
                bool dirty = false;
                foreach (string name in SocketNames)
                {
                    if (!authored.TryGetValue(name, out var a) || !rigNow.TryGetValue(name, out var r))
                        continue;
                    float drift = Vector3.Distance(a.pos, r.pos);
                    sb.AppendLine($"   {name} drift={drift:F4}");
                    if (drift <= DriftTolerance) continue;

                    Transform t = FindIn(rigRoot, name);
                    if (t == null) continue;
                    t.localPosition = a.pos;
                    t.localRotation = a.rot;
                    t.localScale = a.scale;
                    dirty = true;
                    sb.AppendLine($"      -> set to body-authored ({a.pos.x:F4}, {a.pos.y:F4}, {a.pos.z:F4})");
                }
                if (dirty) PrefabUtility.SaveAsPrefabAsset(rigRoot, rigPath);
            }
            finally { PrefabUtility.UnloadPrefabContents(rigRoot); }

            // 3) retarget the constant socket curves so they stop overriding the fix
            int rewritten = RetargetConstantSocketCurves(
                Anim + family + "/" + race + "/Clips", authored, sb);
            sb.AppendLine("   clips with retargeted socket curves: " + rewritten);
        }

        return sb.ToString();
    }

    // Rewrite the constant socket curves to the authored offset. Removing them outright
    // would also remove the (constant) local rotation/scale the clip asserts, and the
    // Animator would then fall back to a mix of clip and prefab state depending on write
    // defaults. Writing the correct constant keeps the clip self-consistent: whatever the
    // Animator applies is the value the body mesh was authored with.
    static int RetargetConstantSocketCurves(
        string clipDir,
        Dictionary<string, SocketTRS> authored,
        StringBuilder sb)
    {
        if (!AssetDatabase.IsValidFolder(clipDir)) return 0;

        int changed = 0;
        foreach (string guid in AssetDatabase.FindAssets("t:AnimationClip", new[] { clipDir }))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            if (clip == null) continue;

            bool dirty = false;
            foreach (var b in AnimationUtility.GetCurveBindings(clip))
            {
                if (!IsSocketPath(b.path)) continue;
                if (b.propertyName.IndexOf("m_LocalPosition", System.StringComparison.Ordinal) != 0
                    && b.propertyName.IndexOf("m_LocalRotation", System.StringComparison.Ordinal) != 0)
                    continue;

                string socket = SocketOf(b.path);
                if (socket == null || !authored.TryGetValue(socket, out var a)) continue;

                AnimationCurve c = AnimationUtility.GetEditorCurve(clip, b);
                if (c == null || c.length == 0) continue;
                if (!IsConstant(c)) continue; // bow/chair/death/social keep real motion

                float target = TargetConstant(b.propertyName, a);
                if (target == float.NaN) continue;
                if (Mathf.Abs(c.keys[0].value - target) < 1e-6f) continue;

                var keys = c.keys;
                for (int i = 0; i < keys.Length; i++)
                    keys[i].value = target;
                AnimationUtility.SetEditorCurve(clip, b, new AnimationCurve(keys));
                dirty = true;
            }

            if (dirty)
            {
                EditorUtility.SetDirty(clip);
                changed++;
            }
        }
        AssetDatabase.SaveAssets();
        return changed;
    }

    static string SocketOf(string path)
    {
        foreach (string n in SocketNames)
            if (path.EndsWith(n, System.StringComparison.Ordinal)) return n;
        return null;
    }

    // One channel (x/y/z or x/y/z/w) of the authored constant.
    static float TargetConstant(string propertyName, SocketTRS a)
    {
        if (propertyName.EndsWith(".x", System.StringComparison.Ordinal))
            return EndsWith(propertyName, "m_LocalPosition") ? a.pos.x : a.rot.x;
        if (propertyName.EndsWith(".y", System.StringComparison.Ordinal))
            return EndsWith(propertyName, "m_LocalPosition") ? a.pos.y : a.rot.y;
        if (propertyName.EndsWith(".z", System.StringComparison.Ordinal))
            return EndsWith(propertyName, "m_LocalPosition") ? a.pos.z : a.rot.z;
        if (propertyName.EndsWith(".w", System.StringComparison.Ordinal))
            return a.rot.w;
        return float.NaN;
    }

    static bool EndsWith(string value, string suffix) =>
        value.IndexOf(suffix, System.StringComparison.Ordinal) == 0;

    static bool IsConstant(AnimationCurve c)
    {
        float first = c.keys[0].value;
        for (int i = 1; i < c.length; i++)
            if (Mathf.Abs(c.keys[i].value - first) > 1e-5f) return false;
        return true;
    }

    static bool IsSocketPath(string path)
    {
        if (string.IsNullOrEmpty(path)) return false;
        foreach (string n in SocketNames)
            if (path.EndsWith(n, System.StringComparison.Ordinal)) return true;
        return false;
    }

    struct SocketTRS { public Vector3 pos; public Quaternion rot; public Vector3 scale; }

    static Dictionary<string, SocketTRS> ReadSockets(string prefabPath)
    {
        var map = new Dictionary<string, SocketTRS>(4);
        var pf = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (pf == null) return map;
        var go = (GameObject)PrefabUtility.InstantiatePrefab(pf);
        try
        {
            foreach (string name in SocketNames)
            {
                Transform t = FindIn(go, name);
                if (t == null) continue;
                map[name] = new SocketTRS { pos = t.localPosition, rot = t.localRotation, scale = t.localScale };
            }
        }
        finally { Object.DestroyImmediate(go); }
        return map;
    }

    static Transform FindIn(GameObject root, string name)
    {
        foreach (var t in root.GetComponentsInChildren<Transform>(true))
            if (t.name == name) return t;
        return null;
    }

    static string FindFirst(params string[] paths)
    {
        foreach (string p in paths)
            if (AssetDatabase.LoadAssetAtPath<GameObject>(p) != null) return p;
        return null;
    }
}
#endif