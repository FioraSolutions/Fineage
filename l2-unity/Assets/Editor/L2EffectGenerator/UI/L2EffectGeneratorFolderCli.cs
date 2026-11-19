#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Headless generate: set L2_EFFECT_FOLDER, L2_EFFECT_NAME, L2_EFFECT_SKILL_ID
/// and run Unity -executeMethod L2EffectGeneratorFolderCli.GenerateFolderCli.
/// Skill ID is the GlobalEffect / skill_visual_effect id (361 = Shock Blast / Mirage art).
/// </summary>
public static class L2EffectGeneratorFolderCli
{
    public static void GenerateFolderCli()
    {
        string folder = Environment.GetEnvironmentVariable("L2_EFFECT_FOLDER");
        string name = Environment.GetEnvironmentVariable("L2_EFFECT_NAME");
        string idRaw = Environment.GetEnvironmentVariable("L2_EFFECT_SKILL_ID");

        if (string.IsNullOrWhiteSpace(folder) || string.IsNullOrWhiteSpace(name))
        {
            Debug.LogError("[L2EffectGenerator] L2_EFFECT_FOLDER and L2_EFFECT_NAME are required.");
            EditorApplication.Exit(1);
            return;
        }

        int skillId = 0;
        if (!string.IsNullOrWhiteSpace(idRaw) &&
            !int.TryParse(idRaw, out skillId))
        {
            Debug.LogError("[L2EffectGenerator] L2_EFFECT_SKILL_ID is not an integer: " + idRaw);
            EditorApplication.Exit(1);
            return;
        }

        L2EffectGeneratorFolderBuilder.BuildResult result =
            L2EffectGeneratorFolderBuilder.CreateFromFolder(folder, name, skillId, true);

        for (int i = 0; i < result.LogLines.Count; i++)
        {
            Debug.Log("[L2EffectGenerator] " + result.LogLines[i]);
        }

        if (!result.Success)
        {
            Debug.LogError("[L2EffectGenerator] " + result.ErrorMessage);
            EditorApplication.Exit(1);
            return;
        }

        Debug.Log("[L2EffectGenerator] GenerateFolderCli ok folder=" + folder + " skill=" + skillId);
    }
}
#endif
