#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// One-shot / on-demand batch: strip redundant Euler curves from committed .anim assets.
/// Menu + CLI for macOS Unity after pull; CI uses scripts/check-anim-rotation-hygiene.py.
/// </summary>
public static class L2AnimClipRotationHygieneBatch
{
    const string AnimRoot = "Assets/Resources/Data/Animations";

    [MenuItem("Fineage/Animations/Strip redundant Euler rotation curves (all)")]
    public static void MenuRun()
    {
        Debug.Log(Run());
    }

    public static void RunCli()
    {
        string result;
        bool ok = true;
        try
        {
            result = Run();
        }
        catch (System.Exception e)
        {
            result = e.ToString();
            ok = false;
        }

        Debug.Log("[AnimRotationHygiene] " + result);
        EditorApplication.Exit(ok ? 0 : 1);
    }

    public static string Run()
    {
        var guids = AssetDatabase.FindAssets("t:AnimationClip", new[] { AnimRoot });
        int touched = 0;
        int curves = 0;
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            if (clip == null)
                continue;
            int n = L2AnimClipPipeline.StripRedundantEulerCurves(clip);
            if (n <= 0)
                continue;
            EditorUtility.SetDirty(clip);
            touched++;
            curves += n;
        }

        if (touched > 0)
            AssetDatabase.SaveAssets();
        return "clips=" + touched + " eulerCurvesRemoved=" + curves + " scanned=" + guids.Length;
    }
}
#endif