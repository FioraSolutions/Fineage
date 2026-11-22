#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Imports MFighter dual-sword SpAtk takes required by Duelist Mix skills (Triple Slash, Sonic line).
/// Original files stay on the Mac. Export, convert, stage, then import:
/// <code>
/// umodel -game=l2 -export -out=$HOME/l2-export/Fighter \
///   "$HOME/Downloads/L2_Interlude_Client/Lineage II/Animations/Fighter.ukx"
/// blender -b -P l2-unity/Assets/Editor/L2EffectGenerator/Blender/psk_psa_to_fbx.py -- \
///   $HOME/l2-export/Fighter/MFighter_m000_b.psk \
///   $HOME/l2-export/Fighter/MFighter_anim.psa \
///   l2-unity/Assets/L2ImportTmp/MFighter_anim.fbx \
///   l2-unity/Assets/Editor/L2EffectGenerator/Blender/io_import_scene_unreal_psa_psk_280.py
/// Unity -batchmode -nographics -quit -projectPath l2-unity \
///   -executeMethod L2MFighterDuelistSpAtkImporter.RunCli
/// </code>
/// See <see cref="L2MFighterSpAtk06Importer"/> for remapping/calibration mechanics.
/// <c>RunCli</c> exits 1 until that FBX is wired the same way; it does not invent a take.
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