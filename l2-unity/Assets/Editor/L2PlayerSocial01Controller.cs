#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

/// <summary>
/// Wires the shared <c>social01</c> gesture state into Player_Basic and race override slots.
/// Runtime swaps the override clip per SocialAction id via <see cref="PlayerSocialAnimApply"/>.
///
///   Unity -batchmode -nographics -quit -projectPath l2-unity \
///     -executeMethod L2PlayerSocial01Controller.RunCli
/// </summary>
public static class L2PlayerSocial01Controller
{
    const string Root = "Assets/Resources/Data/Animations/";
    const string BaseControllerPath = Root + "Animator/_Template/Player_Basic.controller";
    const string PlaceholderClipPath = Root + "Animator/_Template/Player_Basic.ao_social01_placeholder.anim";
    const string OverrideDir = Root + "Animator/Player/";
    const string StateName = InterludeSocialAnimationCatalog.SocialAnimatorStateName;

    static readonly string[][] Races =
    {
        new[] { "MFighter", "MFighter_m000_b.ao_Social_victory_MFighter" },
        new[] { "FFighter", "FFighter_m000_b.ao_Social_victory_FFighter" },
        new[] { "MElf", "MElf_m000_b.ao_Social_victory_MElf" },
        new[] { "FElf", "FElf_m000_b.ao_Social_victory_FElf" },
        new[] { "MMagic", "MMagic_M000_b.ao_Social_victory_MMagic" },
        new[] { "FMagic", "FMagic_m000_b.ao_Social_victory_FMagic" },
        new[] { "MDarkElf", "MDarkElf_m000_b.ao_Social_victory_MDarkElf" },
        new[] { "FDarkElf", "FDarkElf_m000_b.ao_Social_victory_FDarkElf" },
        new[] { "MDwarf", "MDwarf_m000_b.ao_Social_victory_MDwarf" },
        new[] { "FDwarf", "FDwarf_m001_b.ao_Social_victory_FDwarf" },
        new[] { "MOrc", "MOrc_m000_b.ao_Social_victory_MOrc" },
        new[] { "FOrc", "FOrc_m000_b.ao_Social_victory_FOrc" },
        new[] { "MShaman", "MShaman_m000_b.ao_Social_victory_MShaman" },
        new[] { "FShaman", "FShaman_m000_b.ao_Social_victory_FShaman" },
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
        Debug.Log("[Social01Ctl] " + report);
        File.WriteAllText("/tmp/fineage-social01-controller.txt", report);
        EditorApplication.Exit(code);
    }

    [MenuItem("Fineage/Animations/Wire player social01 gesture")]
    public static void Menu() => Debug.Log("[Social01Ctl] " + Run());

    public static string Run()
    {
        var log = new List<string>();
        AnimationClip baseClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(PlaceholderClipPath);
        if (baseClip == null)
            throw new Exception("missing placeholder " + PlaceholderClipPath);

        var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(BaseControllerPath);
        if (controller == null)
            throw new Exception("missing " + BaseControllerPath);

        AnimatorState state = EnsureState(controller, baseClip, log);
        EnsureSocialBehaviour(state, log);
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
            state = machine.AddState(StateName, new Vector3(560f, 120f, 0f));
            log.Add("state added " + StateName);
        }
        else
        {
            log.Add("state exists " + StateName);
        }

        state.motion = motion;
        state.speedParameterActive = false;
        EditorUtility.SetDirty(state);
        return state;
    }

    static void EnsureSocialBehaviour(AnimatorState state, List<string> log)
    {
        PlayerStateSocial behaviour = state.behaviours.OfType<PlayerStateSocial>().FirstOrDefault();
        if (behaviour == null)
        {
            behaviour = state.AddStateMachineBehaviour<PlayerStateSocial>();
            log.Add("behaviour added");
        }
        else
        {
            log.Add("behaviour exists");
        }
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