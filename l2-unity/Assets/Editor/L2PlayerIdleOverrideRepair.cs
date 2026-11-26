#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Ensures wait_hand override slots use catalog wait_Hand clips (not Stand_* one-shots).
/// </summary>
public static class L2PlayerIdleOverrideRepair
{
    const string OverrideRoot = "Assets/Resources/Data/Animations/Animator/Player/";

    public static string RunAll()
    {
        int repaired = 0;
        var log = new List<string>();
        foreach (PlayerLocomotionBodyCatalog.Row row in PlayerLocomotionBodyCatalog.AllRows())
        {
            string ocPath = OverrideRoot + row.Profile + ".overrideController";
            var oc = AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(ocPath);
            if (oc == null)
            {
                log.Add(row.Profile + " missing override " + ocPath);
                continue;
            }

            AnimationClip wait = Resources.Load<AnimationClip>(row.WaitHandClipResources);
            if (wait == null)
            {
                log.Add(row.Profile + " missing wait clip " + row.WaitHandClipResources);
                continue;
            }

            var pairs = new List<KeyValuePair<AnimationClip, AnimationClip>>();
            oc.GetOverrides(pairs);
            bool changed = false;
            for (int i = 0; i < pairs.Count; i++)
            {
                AnimationClip key = pairs[i].Key;
                if (key == null || !key.name.Equals("wait_hand", System.StringComparison.OrdinalIgnoreCase))
                    continue;

                AnimationClip current = pairs[i].Value;
                if (current == wait)
                    continue;

                if (current != null && current.name.ToLowerInvariant().Contains("stand"))
                {
                    pairs[i] = new KeyValuePair<AnimationClip, AnimationClip>(key, wait);
                    changed = true;
                    repaired++;
                    log.Add(row.Profile + " wait_hand " + current.name + " -> " + wait.name);
                }
            }

            if (changed)
            {
                oc.ApplyOverrides(pairs);
                EditorUtility.SetDirty(oc);
            }
        }

        if (repaired > 0)
            AssetDatabase.SaveAssets();

        return "idle override repair: fixed=" + repaired + " " + string.Join("; ", log);
    }

    [MenuItem("Fineage/Player/Repair wait_hand idle overrides")]
    public static void MenuRun() => Debug.Log(RunAll());
}
#endif