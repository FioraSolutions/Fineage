#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Shared character animation clip post-steps for PSA / FBX → standalone .anim export.
/// Importers and batch hygiene call this — no per-race one-offs.
/// </summary>
public static class L2AnimClipPipeline
{
    /// <summary>
    /// PSA/FBX conversion often emits both quaternion and Euler rotation curves on the same bone.
    /// Unity evaluates both, which shows up as idle foot slide / sway while the root stays put.
    /// Prefer quaternion curves (Unreal export path).
    /// </summary>
    public static int StripRedundantEulerCurves(AnimationClip clip)
    {
        if (clip == null)
            return 0;

        EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(clip);
        var quatPaths = new HashSet<string>();
        for (int i = 0; i < bindings.Length; i++)
        {
            EditorCurveBinding b = bindings[i];
            if (b.propertyName.StartsWith("m_LocalRotation.", System.StringComparison.Ordinal))
                quatPaths.Add(b.path);
        }

        if (quatPaths.Count == 0)
            return 0;

        int removed = 0;
        for (int i = 0; i < bindings.Length; i++)
        {
            EditorCurveBinding b = bindings[i];
            if (!b.propertyName.StartsWith("localEulerAngles.", System.StringComparison.Ordinal))
                continue;
            if (!quatPaths.Contains(b.path))
                continue;
            AnimationUtility.SetEditorCurve(clip, b, null);
            removed++;
        }

        return removed;
    }

    public static void ApplyStandardLoopSettings(AnimationClip clip, bool loop)
    {
        if (clip == null)
            return;
        AnimationClipSettings st = AnimationUtility.GetAnimationClipSettings(clip);
        st.loopTime = loop;
        st.loopBlend = false;
        st.loopBlendOrientation = false;
        st.loopBlendPositionY = false;
        st.loopBlendPositionXZ = false;
        AnimationUtility.SetAnimationClipSettings(clip, st);
    }

    /// <summary>True when any bone on <paramref name="clip"/> has both quaternion and Euler rotation curves.</summary>
    public static bool HasRedundantEulerCurves(AnimationClip clip)
    {
        if (clip == null)
            return false;

        EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(clip);
        var quatPaths = new HashSet<string>();
        for (int i = 0; i < bindings.Length; i++)
        {
            EditorCurveBinding b = bindings[i];
            if (b.propertyName.StartsWith("m_LocalRotation.", System.StringComparison.Ordinal))
                quatPaths.Add(b.path);
        }

        if (quatPaths.Count == 0)
            return false;

        for (int i = 0; i < bindings.Length; i++)
        {
            EditorCurveBinding b = bindings[i];
            if (b.propertyName.StartsWith("localEulerAngles.", System.StringComparison.Ordinal)
                && quatPaths.Contains(b.path))
                return true;
        }

        return false;
    }
}
#endif