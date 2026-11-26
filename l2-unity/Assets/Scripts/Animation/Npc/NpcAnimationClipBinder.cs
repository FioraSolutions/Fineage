using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data-driven NPC / monster locomotion + weapon overrides via AnimTween + InterludeAnimationClipResolver.
/// </summary>
public static class NpcAnimationClipBinder
{
    static readonly string[] Slots = { "atk01", "atkwait", "wait", "walk", "run" };

    public static void Bind(NetworkAnimationController controller, Entity entity)
    {
        if (controller == null || entity == null)
            return;

        Animator animator = controller.GetAnimator();
        if (animator == null)
            return;

        AnimatorOverrideController oc = animator.runtimeAnimatorController as AnimatorOverrideController;
        if (oc == null)
            return;

        string meshStem = ResolveMeshStem(entity, oc);
        if (string.IsNullOrEmpty(meshStem))
            return;

        string meshAnim = InterludeAnimationClipResolver.DeriveMeshAnimKey(meshStem);
        string weaponSuffix = WeaponSuffix(entity);

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

            AnimationClip resolved = InterludeAnimationClipResolver.TryLoadCreatureLocomotion(
                meshStem,
                meshAnim,
                slot,
                weaponSuffix);
            if (resolved != null)
                oc[original] = resolved;
        }
    }

    static string ResolveMeshStem(Entity entity, AnimatorOverrideController oc)
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

    static string WeaponSuffix(Entity entity)
    {
        if (entity.Gear != null && !string.IsNullOrEmpty(entity.Gear.WeaponAnim))
            return entity.Gear.WeaponAnim;

        int rhand = entity.Appearance != null ? entity.Appearance.RHand : 0;
        if (rhand <= 0)
            return null;
        CombatStyleProfile style = CombatStyleEntity.ResolveFromItemId(rhand);
        return style.IsResolved ? style.AnimSuffix : null;
    }

    static string DetectSlot(string clipName) => CreatureWeaponAnimApplyDetectSlot(clipName);

    // Reuse slot detection rules without duplicating heuristic clip folders.
    static string CreatureWeaponAnimApplyDetectSlot(string clipName)
    {
        if (string.IsNullOrEmpty(clipName))
            return null;
        string n = clipName.ToLowerInvariant();
        for (int i = 0; i < Slots.Length; i++)
        {
            string slot = Slots[i];
            if (n.Equals(slot, StringComparison.Ordinal)
                || n.Contains("_" + slot)
                || n.Contains(".ao_" + slot))
                return slot;
        }
        return null;
    }
}