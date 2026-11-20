using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Swaps the race override clip behind Player_Basic <c>social01</c> before a social gesture plays.
/// </summary>
public static class PlayerSocialAnimApply
{
    const string SocialState = "social01";

    public static bool TryPrepareSocialClip(Entity entity, int socialActionId, out string stateToPlay)
    {
        stateToPlay = SocialState;
        if (entity == null)
            return false;

        if (!InterludeSocialAnimationCatalog.TryGetClipStem(socialActionId, out string clipStem))
            return false;

        CharacterRaceAnimation race = entity.RaceId;
        if (!InterludeSocialAnimationCatalog.TryLoadClip(race, clipStem, out AnimationClip clip))
        {
            Debug.LogError(
                "[SocialAnim] action id=" + socialActionId + " race=" + race +
                " gender=" + InterludeSocialAnimationCatalog.GenderLabel(race) +
                " missing clip stem '" + clipStem + "' under " +
                InterludeSocialAnimationCatalog.ClipResourcesFolder(race) +
                " expected *" + InterludeSocialAnimationCatalog.ExpectedClipFileSubstring(race, clipStem) + "*");
            return false;
        }

        NetworkAnimationController nac = entity.GetAnimatorController();
        Animator animator = nac != null ? nac.GetAnimator() : entity.Animator;
        if (animator == null)
            return false;

        AnimatorOverrideController oc = animator.runtimeAnimatorController as AnimatorOverrideController;
        if (oc == null)
            return false;

        var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>(oc.overridesCount);
        oc.GetOverrides(overrides);
        bool bound = false;
        for (int i = 0; i < overrides.Count; i++)
        {
            AnimationClip original = overrides[i].Key;
            if (original == null)
                continue;
            string on = original.name;
            if (on.IndexOf(SocialState, StringComparison.OrdinalIgnoreCase) < 0
                && overrides[i].Value != null
                && overrides[i].Value.name.IndexOf(SocialState, StringComparison.OrdinalIgnoreCase) < 0)
                continue;

            oc[original] = clip;
            bound = true;
            break;
        }

        if (!bound)
        {
            Debug.LogError(
                "[SocialAnim] action id=" + socialActionId + " race=" + race +
                " could not find social01 override slot on animator.");
            return false;
        }

        return true;
    }
}