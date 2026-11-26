#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

/// <summary>
/// Wires the retail self-buff gesture <c>SpAtk06_hand</c> into the shared player controller.
///
/// <c>AnimationCombo_Interlude.txt</c> maps the physical self-skill tag <c>X</c> (Ultimate Evasion,
/// Dash, Focus Chance, Escape Shackle) to <c>SpAtk06AnimName</c>. In the Interlude data that
/// sequence is <c>SpAtk06_Hand_&lt;Race&gt;</c> for every race — a hands-up buff pose with no
/// weapon and no hit/shoot notifies — but <c>Player_Basic.controller</c> only defined
/// <c>SpAtk01/02</c> states, so the runner fell back to <c>SpAtk01_1HS</c>: Dash played a sword
/// swing and its AttackShot notify emitted a melee hit on the selected mob.
///
/// Adds the missing state on the template controller (motion = the template's donor clip, which is
/// the key every override controller replaces) and points each race override at that race's own
/// <c>Clips/*_ao_SpAtk06_Hand_*.anim</c>.
///
///   Unity -batchmode -nographics -quit -projectPath l2-unity \
///     -executeMethod L2PlayerSpAtk06Controller.RunCli
/// </summary>
public static class L2PlayerSpAtk06Controller
{
    const string Root = "Assets/Resources/Data/Animations/";
    const string BaseControllerPath = Root + "Animator/_Template/Player_Basic.controller";
    const string OverrideDir = Root + "Animator/Player/";
    const string StateName = "SpAtk06_hand";
    /// <summary>Donor clip already shipped by the template controller for the other SpAtk states.</summary>
    const string BaseClipStem = "FDarkElf_m000_b.ao_SpAtk06_Hand_FDarkElf";

    /// <summary>race id -> that race's own SpAtk06_Hand clip stem.</summary>
    static readonly string[][] Races =
    {
        new[] { "MFighter", "MFighter_m000_b.ao_SpAtk06_Hand_MFighter" },
        new[] { "FFighter", "FFighter_m000_b.ao_SpAtk06_Hand_FFighter" },
        new[] { "MElf", "MElf_m000_b.ao_SpAtk06_Hand_MElf" },
        new[] { "FElf", "FElf_m000_b.ao_SpAtk06_Hand_FElf" },
        new[] { "MMagic", "MMagic_M000_b.ao_SpAtk06_Hand_MMagic" },
        new[] { "FMagic", "FMagic_m000_b.ao_SpAtk06_Hand_FMagic" },
        new[] { "MDarkElf", "MDarkElf_m000_b.ao_SpAtk06_Hand_MDarkElf" },
        new[] { "FDarkElf", "FDarkElf_m000_b.ao_SpAtk06_Hand_FDarkElf" },
        new[] { "MDwarf", "MDwarf_m000_b.ao_SpAtk06_Hand_MDwarf" },
        new[] { "FDwarf", "FDwarf_m001_b.ao_SpAtk06_Hand_FDwarf" },
        new[] { "MOrc", "MOrc_m000_b.ao_SpAtk06_Hand_MOrc" },
        new[] { "FOrc", "FOrc_m000_b.ao_SpAtk06_Hand_FOrc" },
        new[] { "MShaman", "MShaman_m000_b.ao_SpAtk06_Hand_MShaman" },
        new[] { "FShaman", "FShaman_m000_b.ao_SpAtk06_Hand_FShaman" },
    };

    public static void RunCli()
    {
        string report;
        int code = 0;
        try
        {
            report = Run();
        }
        catch (Exception error)
        {
            report = error.ToString();
            code = 1;
        }
        Debug.Log("[SpAtk06Ctl] " + report);
        File.WriteAllText("/tmp/fineage-spatk06-controller.txt", report);
        EditorApplication.Exit(code);
    }

    [MenuItem("Fineage/Animations/Wire SpAtk06 buff gesture")]
    public static void Menu() => Debug.Log("[SpAtk06Ctl] " + Run());

    public static string Run()
    {
        var log = new List<string>();
        var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(BaseControllerPath);
        if (controller == null)
            throw new Exception("missing " + BaseControllerPath);

        AnimationClip baseClip = FindClip(BaseClipStem);
        if (baseClip == null)
            throw new Exception("missing donor clip " + BaseClipStem);

        AnimatorState state = EnsureState(controller, baseClip, log);
        EnsureBuffBehaviour(state, log);
        EditorUtility.SetDirty(controller);
        AssetDatabase.SaveAssets();

        int wired = 0;
        foreach (string[] race in Races)
        {
            AnimationClip raceClip = FindClip(race[1]);
            if (raceClip == null)
            {
                log.Add(race[0] + ": clip missing " + race[1]);
                continue;
            }

            string path = OverrideDir + race[0] + ".overrideController";
            var oc = AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(path);
            if (oc == null)
            {
                log.Add(race[0] + ": no override controller");
                continue;
            }

            if (oc.runtimeAnimatorController != controller)
                oc.runtimeAnimatorController = controller;

            var pairs = new List<KeyValuePair<AnimationClip, AnimationClip>>();
            oc.GetOverrides(pairs);
            int index = pairs.FindIndex(p => p.Key == baseClip);
            if (index >= 0)
                pairs[index] = new KeyValuePair<AnimationClip, AnimationClip>(baseClip, raceClip);
            else
                pairs.Add(new KeyValuePair<AnimationClip, AnimationClip>(baseClip, raceClip));

            oc.ApplyOverrides(pairs);
            EditorUtility.SetDirty(oc);
            wired++;
        }

        AssetDatabase.SaveAssets();
        log.Add("override controllers wired " + wired + "/" + Races.Length);
        return string.Join(" | ", log);
    }

    static AnimatorState EnsureState(AnimatorController controller, AnimationClip motion, List<string> log)
    {
        AnimatorStateMachine machine = controller.layers[0].stateMachine;
        AnimatorState state = machine.states
            .Select(s => s.state)
            .FirstOrDefault(s => s != null && s.name == StateName);

        if (state == null)
        {
            state = machine.AddState(StateName, new Vector3(560f, 60f, 0f));
            log.Add("state added " + StateName);
        }
        else
        {
            log.Add("state exists " + StateName);
        }

        state.motion = motion;
        state.speedParameterActive = true;
        state.speedParameter = "patkspd";
        EditorUtility.SetDirty(state);
        return state;
    }

    /// <summary>
    /// SpAtk06 is a self buff: the retail clip has no hit window. Leaving the two event names empty
    /// keeps the SMB on the authored playback rate and still releases the skill runner, because the
    /// state name it reports at the end is <c>SpAtk06_hand</c> — the same name the runner awaits.
    /// </summary>
    static void EnsureBuffBehaviour(AnimatorState state, List<string> log)
    {
        PlayerStateSpAtk behaviour = state.behaviours.OfType<PlayerStateSpAtk>().FirstOrDefault();
        if (behaviour == null)
        {
            behaviour = state.AddStateMachineBehaviour<PlayerStateSpAtk>();
            log.Add("behaviour added");
        }
        else
        {
            log.Add("behaviour exists");
        }

        behaviour.motionName = BaseClipStem;
        behaviour.eventStartHitName = string.Empty;
        behaviour.eventEndHitName = string.Empty;
        EditorUtility.SetDirty(behaviour);
        EditorUtility.SetDirty(state);
    }

    static AnimationClip FindClip(string stem)
    {
        foreach (string guid in AssetDatabase.FindAssets(stem + " t:AnimationClip", new[] { Root }))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (Path.GetFileNameWithoutExtension(path) == stem)
                return AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
        }

        return null;
    }
}
#endif
