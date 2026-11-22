using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Binds Npc_Basic / Monster_Basic wait/walk/run override slots from shipped .ao clips
/// (dryad, orc_fighter, giant_spider, wolf, etc.) when import left null or wrong stems.
/// </summary>
public static class NpcLocomotionClipBootstrap
{
    static readonly string[] LocomotionSlots = { "wait", "walk", "run", "atkwait" };
    static readonly string[] Packages =
    {
        "LineageMonsters",
        "LineageNPCs",
        "LineageNPCs4",
        "LineageNpcs",
    };

    static readonly Dictionary<string, AnimationClip[]> ClipCache =
        new Dictionary<string, AnimationClip[]>(StringComparer.OrdinalIgnoreCase);

    static readonly HashSet<string> LoggedMissing =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    public static void Bind(NetworkAnimationController controller, Entity entity)
    {
        if (controller == null)
            return;

        Animator animator = controller.GetAnimator();
        if (animator == null)
            return;

        AnimatorOverrideController oc = animator.runtimeAnimatorController as AnimatorOverrideController;
        if (oc == null)
            return;

        string prefix = ResolveClipPrefix(oc, entity);
        if (string.IsNullOrEmpty(prefix))
            return;

        AnimationClip[] clips = LoadClips(prefix);
        if (clips == null || clips.Length == 0)
            return;

        var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>(oc.overridesCount);
        oc.GetOverrides(overrides);
        bool any = false;
        for (int i = 0; i < overrides.Count; i++)
        {
            AnimationClip original = overrides[i].Key;
            if (original == null)
                continue;

            string slot = DetectSlot(original.name);
            if (slot == null && overrides[i].Value != null)
                slot = DetectSlot(overrides[i].Value.name);
            if (slot == null)
                continue;

            AnimationClip resolved = FindLocomotionClip(clips, prefix, slot);
            if (resolved == null)
                continue;

            oc[original] = resolved;
            any = true;
        }

        if (!any && LoggedMissing.Add(prefix))
        {
            Debug.LogError(
                "[NpcLocomotion] could not bind wait/walk/run prefix="
                + prefix
                + " clips="
                + clips.Length
                + " npcId="
                + (entity != null && entity.Identity != null ? entity.Identity.NpcId.ToString() : "0"));
        }
    }

    static string ResolveClipPrefix(AnimatorOverrideController oc, Entity entity)
    {
        if (entity?.Identity != null)
        {
            Npcgrp grp = TablePacketServices.NpcGrps?.GetNpcgrp(entity.Identity.NpcId);
            if (grp != null && !string.IsNullOrEmpty(grp.Mesh))
            {
                string mesh = grp.Mesh;
                int dot = mesh.LastIndexOf('.');
                if (dot > 0)
                    mesh = mesh.Substring(dot + 1);
                int slash = Math.Max(mesh.LastIndexOf('/'), mesh.LastIndexOf('\\'));
                if (slash >= 0 && slash < mesh.Length - 1)
                    mesh = mesh.Substring(slash + 1);
                if (!string.IsNullOrEmpty(mesh))
                    return mesh.ToLowerInvariant();
            }
        }

        var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>(oc.overridesCount);
        oc.GetOverrides(overrides);
        for (int i = 0; i < overrides.Count; i++)
        {
            string name = overrides[i].Value != null ? overrides[i].Value.name : overrides[i].Key?.name;
            string prefix = PrefixBeforeAo(name);
            if (prefix != null)
                return prefix;
        }

        return PrefixBeforeAo(oc.name);
    }

    static string PrefixBeforeAo(string clipName)
    {
        if (string.IsNullOrEmpty(clipName))
            return null;
        int ao = clipName.IndexOf(".ao_", StringComparison.OrdinalIgnoreCase);
        if (ao > 0)
            return clipName.Substring(0, ao).ToLowerInvariant();
        return null;
    }

    static AnimationClip[] LoadClips(string prefix)
    {
        if (ClipCache.TryGetValue(prefix, out AnimationClip[] cached))
            return cached;

        for (int i = 0; i < Packages.Length; i++)
        {
            string folder = "Data/Animations/" + Packages[i] + "/" + prefix;
            AnimationClip[] root = Resources.LoadAll<AnimationClip>(folder);
            AnimationClip[] model = Resources.LoadAll<AnimationClip>(folder + "/Model");
            if ((root != null && root.Length > 0) || (model != null && model.Length > 0))
            {
                int count = (root?.Length ?? 0) + (model?.Length ?? 0);
                var merged = new AnimationClip[count];
                int w = 0;
                if (root != null)
                {
                    for (int r = 0; r < root.Length; r++)
                        merged[w++] = root[r];
                }
                if (model != null)
                {
                    for (int m = 0; m < model.Length; m++)
                        merged[w++] = model[m];
                }

                ClipCache[prefix] = merged;
                return merged;
            }
        }

        if (LoggedMissing.Add("load:" + prefix))
        {
            Debug.LogError(
                "[NpcLocomotion] missing animation folder prefix="
                + prefix
                + " under "
                + string.Join(",", Packages));
        }

        ClipCache[prefix] = Array.Empty<AnimationClip>();
        return ClipCache[prefix];
    }

    static string DetectSlot(string clipName)
    {
        if (string.IsNullOrEmpty(clipName))
            return null;
        string n = clipName.ToLowerInvariant();
        for (int i = 0; i < LocomotionSlots.Length; i++)
        {
            string slot = LocomotionSlots[i];
            if (n.Equals(slot, StringComparison.Ordinal)
                || n.EndsWith("_" + slot, StringComparison.Ordinal)
                || n.Contains(".ao_" + slot, StringComparison.Ordinal)
                || n.Contains(".ao_" + Capitalize(slot), StringComparison.Ordinal))
                return slot;
        }
        return null;
    }

    static string Capitalize(string slot)
    {
        if (string.IsNullOrEmpty(slot))
            return slot;
        if (slot.Length == 1)
            return slot.ToUpperInvariant();
        return char.ToUpperInvariant(slot[0]) + slot.Substring(1);
    }

    static AnimationClip FindLocomotionClip(AnimationClip[] clips, string prefix, string slot)
    {
        string[] patterns =
        {
            prefix + ".ao_" + slot,
            prefix + ".ao_" + Capitalize(slot),
            ".ao_" + slot,
            ".ao_" + Capitalize(slot),
            "_" + slot + ".",
        };

        for (int p = 0; p < patterns.Length; p++)
        {
            string pattern = patterns[p];
            for (int i = 0; i < clips.Length; i++)
            {
                AnimationClip clip = clips[i];
                if (clip == null)
                    continue;
                string name = clip.name;
                if (name.IndexOf(pattern, StringComparison.OrdinalIgnoreCase) >= 0)
                    return clip;
            }
        }

        return null;
    }
}