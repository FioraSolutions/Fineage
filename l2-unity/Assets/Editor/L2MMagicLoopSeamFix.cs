using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// MMagic idle/walk/run/atkWait loops jerk at the wrap: UE interpolates last->first, Unity stops at the last key.
/// Same fix as MElf (907de8cf5 CloseLoop): append the first pose one frame after the end. Idempotent; auto-runs once.
/// </summary>
[InitializeOnLoad]
public static class L2MMagicLoopSeamFix
{
    const string Folder = "Assets/Resources/Data/Animations/Magic/MMagic/Clips";
    const string DoneKey = "Fineage.MMagicLoopSeamFix.v1";

    static L2MMagicLoopSeamFix()
    {
        if (EditorPrefs.GetBool(DoneKey, false)) return;
        EditorApplication.delayCall += () =>
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling) return;
            Run();
            EditorPrefs.SetBool(DoneKey, true);
        };
    }

    static bool IsLoop(string file)
    {
        string n = Path.GetFileNameWithoutExtension(file).ToLowerInvariant();
        int i = n.IndexOf(".ao_"); if (i >= 0) n = n.Substring(i + 4);
        return n.StartsWith("wait_") || n.StartsWith("atkwait_") || n.StartsWith("walk_") || n.StartsWith("run_")
            || n.StartsWith("sitwait") || n.StartsWith("deathwait") || n.StartsWith("swimwait") || n.StartsWith("swim_");
    }

    [MenuItem("Fineage/Animations/Fix MMagic loop seams")]
    public static void Run()
    {
        int fixedN = 0, skipped = 0;
        foreach (string path in Directory.GetFiles(Folder, "*.anim"))
        {
            if (!IsLoop(path)) continue;
            var c = AssetDatabase.LoadAssetAtPath<AnimationClip>(path.Replace('\\', '/'));
            if (c == null) continue;
            if (IsClosed(c)) { skipped++; continue; }
            CloseLoop(c, 1f / Mathf.Max(1f, c.frameRate));
            var st = AnimationUtility.GetAnimationClipSettings(c);
            st.loopTime = true; st.loopBlend = false;
            AnimationUtility.SetAnimationClipSettings(c, st);
            EditorUtility.SetDirty(c);
            fixedN++;
        }
        AssetDatabase.SaveAssets();
        Debug.Log("[MMagicLoopSeam] closed " + fixedN + " loops, already closed " + skipped);
    }

    static bool IsClosed(AnimationClip c)
    {
        foreach (var b in AnimationUtility.GetCurveBindings(c))
        {
            if (b.propertyName.StartsWith("m_LocalRotation")) continue; // sign may be flipped
            var cv = AnimationUtility.GetEditorCurve(c, b);
            if (cv.length > 1 && Mathf.Abs(cv[0].value - cv[cv.length - 1].value) > 1e-4f) return false;
        }
        return true;
    }

    static void CloseLoop(AnimationClip c, float dt)
    {
        var binds = AnimationUtility.GetCurveBindings(c);
        float end = 0f;
        foreach (var b in binds) { var cv = AnimationUtility.GetEditorCurve(c, b); if (cv.length > 0) end = Mathf.Max(end, cv[cv.length - 1].time); }
        float t = end + dt;
        var flip = new Dictionary<string, bool>();
        foreach (var g in binds.Where(b => b.propertyName.StartsWith("m_LocalRotation")).GroupBy(b => b.path))
        {
            float dot = 0f;
            foreach (var b in g) { var cv = AnimationUtility.GetEditorCurve(c, b); if (cv.length > 0) dot += cv[0].value * cv[cv.length - 1].value; }
            flip[g.Key] = dot < 0f;
        }
        foreach (var b in binds)
        {
            var cv = AnimationUtility.GetEditorCurve(c, b); if (cv.length == 0) continue;
            float v = cv[0].value;
            if (b.propertyName.StartsWith("m_LocalRotation") && flip.TryGetValue(b.path, out var f) && f) v = -v;
            cv.AddKey(new Keyframe(t, v));
            AnimationUtility.SetEditorCurve(c, b, cv);
        }
    }
}
