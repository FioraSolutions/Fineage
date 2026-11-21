using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Npc_Basic / Monster_Basic only have bare states (wait / walk / run / atk01 / atkwait).
/// Town guards and other NPCs ship weapon-suffixed clips (atk01_bow, wait_1HS, …) and a
/// baked override that usually binds the 1HS set. Rebind those slots from the pawn's
/// equipped weapon (rhand / Gear.WeaponAnim) so a bow guard shoots instead of swinging.
/// </summary>
public static class CreatureWeaponAnimApply
{
    static readonly string[] Slots = { "atk01", "atkwait", "wait", "walk", "run" };
    static readonly string[] Packages = { "LineageNPCs", "LineageNPCs4", "LineageNpcs", "LineageMonsters" };
    static readonly Dictionary<string, AnimationClip[]> ClipCache = new Dictionary<string, AnimationClip[]>(StringComparer.OrdinalIgnoreCase);
    static readonly HashSet<string> LoggedMissingClipPrefixes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    public static void Bind(Entity entity)
    {
        if (entity == null || entity is UserEntity || entity is PlayerEntity)
            return;

        if (!EntityHasWeaponSlots(entity))
            return;

        string suffix = WeaponSuffix(entity);
        if (string.IsNullOrEmpty(suffix))
            return;

        NetworkAnimationController nac = entity.GetAnimatorController();
        Animator animator = nac != null ? nac.GetAnimator() : entity.Animator;
        if (animator == null)
            return;

        AnimatorOverrideController oc = animator.runtimeAnimatorController as AnimatorOverrideController;
        if (oc == null)
            return;

        AnimationClip[] clips = LoadModelClips(oc);
        if (clips == null || clips.Length == 0)
            return;

        var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>(oc.overridesCount);
        oc.GetOverrides(overrides);
        for (int i = 0; i < overrides.Count; i++)
        {
            AnimationClip original = overrides[i].Key;
            if (original == null)
                continue;
            string hint = overrides[i].Value != null ? overrides[i].Value.name : original.name;
            string slot = DetectSlot(hint) ?? DetectSlot(original.name);
            if (slot == null)
                continue;
            AnimationClip next = FindClip(clips, slot, suffix);
            if (next != null)
                oc[original] = next;
        }
    }

    static string WeaponSuffix(Entity entity)
    {
        if (entity.Gear != null && !string.IsNullOrEmpty(entity.Gear.WeaponAnim))
            return entity.Gear.WeaponAnim;

        int rhand = entity.Appearance != null ? entity.Appearance.RHand : 0;
        if (rhand <= 0)
            return null;
        CombatStyleProfile style = CombatStyleEntity.ResolveFromItemId(rhand);
        if (style.IsResolved)
            return style.AnimSuffix;

        return null;
    }

    static AnimationClip[] LoadModelClips(AnimatorOverrideController oc)
    {
        string prefix = ClipPrefix(oc);
        if (string.IsNullOrEmpty(prefix))
            return Array.Empty<AnimationClip>();

        AnimationClip[] cached;
        if (ClipCache.TryGetValue(prefix, out cached))
            return cached;

        for (int i = 0; i < Packages.Length; i++)
        {
            string folder = "Data/Animations/" + Packages[i] + "/" + prefix + "/Model";
            AnimationClip[] loaded = Resources.LoadAll<AnimationClip>(folder);
            if (loaded != null && loaded.Length > 0)
            {
                ClipCache[prefix] = loaded;
                return loaded;
            }
        }

        if (LoggedMissingClipPrefixes.Add(prefix))
        {
            Debug.LogWarning(
                "[CreatureWeaponAnim] no weapon animation clips prefix=" + prefix
                + " packages=" + string.Join(",", Packages) + "/Model.");
        }

        ClipCache[prefix] = Array.Empty<AnimationClip>();
        return ClipCache[prefix];
    }

    static bool EntityHasWeaponSlots(Entity entity)
    {
        if (entity == null || entity.Appearance == null)
            return false;

        if (entity.Appearance.RHand > 0 || entity.Appearance.LHand > 0)
            return true;

        NpcEntity npc = entity as NpcEntity;
        Npcgrp grp = npc != null && npc.NpcData != null ? npc.NpcData.Npcgrp : null;
        return grp != null && (grp.RHand > 0 || grp.LHand > 0);
    }

    static string ClipPrefix(AnimatorOverrideController oc)
    {
        var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>(oc.overridesCount);
        oc.GetOverrides(overrides);
        for (int i = 0; i < overrides.Count; i++)
        {
            string name = overrides[i].Value != null ? overrides[i].Value.name : null;
            string prefix = PrefixOf(name);
            if (prefix != null)
                return prefix;
            prefix = PrefixOf(overrides[i].Key != null ? overrides[i].Key.name : null);
            if (prefix != null)
                return prefix;
        }
        return PrefixOf(oc.name);
    }

    static string PrefixOf(string clipName)
    {
        if (string.IsNullOrEmpty(clipName))
            return null;
        int ao = clipName.IndexOf(".ao_", StringComparison.OrdinalIgnoreCase);
        if (ao > 0)
            return clipName.Substring(0, ao);
        return null;
    }

    static string DetectSlot(string clipName)
    {
        if (string.IsNullOrEmpty(clipName))
            return null;
        string n = clipName.ToLowerInvariant();
        // Longer tokens first so atkwait is not read as wait.
        for (int i = 0; i < Slots.Length; i++)
        {
            if (HasToken(n, Slots[i], bareWait: Slots[i] == "wait"))
                return Slots[i];
        }
        return null;
    }

    static AnimationClip FindClip(AnimationClip[] clips, string slot, string suffix)
    {
        for (int i = 0; i < clips.Length; i++)
        {
            AnimationClip clip = clips[i];
            if (clip != null && HasToken(clip.name.ToLowerInvariant(), slot + "_" + suffix.ToLowerInvariant(), bareWait: slot == "wait"))
                return clip;
        }
        return null;
    }

    static bool HasToken(string lowerName, string token, bool bareWait = false)
    {
        if (string.IsNullOrEmpty(lowerName) || string.IsNullOrEmpty(token))
            return false;
        int i = 0;
        while (true)
        {
            i = lowerName.IndexOf(token, i, StringComparison.Ordinal);
            if (i < 0)
                return false;
            if (bareWait && IsPrefixedWait(lowerName, i))
            {
                i += token.Length;
                continue;
            }
            int end = i + token.Length;
            if (end < lowerName.Length && char.IsLetterOrDigit(lowerName[end]))
            {
                i += token.Length;
                continue;
            }
            return true;
        }
    }

    static bool IsPrefixedWait(string lowerName, int index)
    {
        // atkwait / deathwait / spwait / jumpwait should not match the idle "wait" slot.
        if (index >= 3 && lowerName.Substring(index - 3, 3) == "atk")
            return true;
        if (index >= 5 && lowerName.Substring(index - 5, 5) == "death")
            return true;
        if (index >= 2 && lowerName.Substring(index - 2, 2) == "sp")
            return true;
        if (index >= 4 && lowerName.Substring(index - 4, 4) == "jump")
            return true;
        return false;
    }
}
