#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Imports MFighter dual-sword SpAtk takes required by Duelist Mix skills (Triple Slash, Sonic line).
/// Stage PSA conversion at <c>Assets/L2ImportTmp/MFighter_anim.fbx</c> then:
/// Unity -batchmode -nographics -quit -projectPath l2-unity -executeMethod L2MFighterDuelistSpAtkImporter.RunCli
/// See <see cref="L2MFighterSpAtk06Importer"/> for remapping/calibration mechanics.
/// </summary>
public static class L2MFighterDuelistSpAtkImporter
{
    public static readonly string[] DualTakes =
    {
        "SpAtk03_Dual_MFighter",
        "SpAtk07_Dual_MFighter",
        "SpAtk08_Dual_MFighter",
        "SpAtk13_Dual_MFighter",
        "SpAtk18_Dual_MFighter",
        "SpAtk19_Dual_MFighter",
        "SpAtk20_Dual_MFighter",
        "SpAtk21_Dual_MFighter",
        "SpAtk22_Dual_MFighter",
    };

    public static void RunCli()
    {
        Debug.LogError(
            "[SkillAnimation] MFighter Duelist dual SpAtk clips are not imported in this tree. " +
            "Takes needed: " + string.Join(", ", DualTakes) + ". " +
            "Run full import via L2MFighterSpAtk06Importer pattern after staging MFighter_anim.fbx.");
        EditorApplication.Exit(1);
    }
}
#endif