#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// Builds Interlude move-click marker LineageEffect.e_u093_a into Resources.
/// </summary>
public static class L2EffectBuildMoveClick
{
    const string EffectRoot = "Assets/Resources/Data/Effects/locator";
    const string EffectName = "e_u093_a";

    [MenuItem("Tools/L2 Effects/Build Move Click (e_u093_a)")]
    public static void BuildFromMenu()
    {
        Build(logToConsole: true);
    }

    /// <summary>Unity -batchmode -executeMethod L2EffectBuildMoveClick.BuildBatch</summary>
    public static void BuildBatch()
    {
        bool ok = Build(logToConsole: true);
        EditorApplication.Exit(ok ? 0 : 1);
    }

    public static bool Build(bool logToConsole)
    {
        L2EffectGeneratorFolderBuilder.BuildResult result =
            L2EffectGeneratorFolderBuilder.CreateFromFolder(
                EffectRoot,
                EffectName,
                preferredSkillId: 0,
                bindToGlobalEffect: false);

        if (logToConsole)
        {
            for (int i = 0; i < result.LogLines.Count; i++)
                Debug.Log("[e_u093_a] " + result.LogLines[i]);
            if (!result.Success)
                Debug.LogError("[e_u093_a] " + result.ErrorMessage);
            else
                Debug.Log("[e_u093_a] build ok");
        }

        return result.Success;
    }
}
#endif
