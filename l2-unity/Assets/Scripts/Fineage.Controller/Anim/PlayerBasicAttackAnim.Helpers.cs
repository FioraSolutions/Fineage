using System;
using System.Collections.Generic;
using UnityEngine;

public sealed partial class PlayerBasicAttackAnim : PlayerAnimStateMapBase
{
    public static bool IsSwingClipName(string clipName)
    {
        if (string.IsNullOrEmpty(clipName))
            return false;
        if (clipName.IndexOf("wait", StringComparison.OrdinalIgnoreCase) >= 0)
            return false;
        if (clipName.IndexOf("run", StringComparison.OrdinalIgnoreCase) >= 0)
            return false;
        if (clipName.IndexOf("walk", StringComparison.OrdinalIgnoreCase) >= 0)
            return false;
        return clipName.IndexOf("atk", StringComparison.OrdinalIgnoreCase) >= 0
            || clipName.IndexOf("jatk", StringComparison.OrdinalIgnoreCase) >= 0;
    }

    public static bool TryGetSwingClipLength(
        Animator animator,
        int layer,
        out float length,
        out string clipName)
    {
        length = 0f;
        clipName = null;
        if (animator == null)
            return false;

        AnimatorClipInfo[] clips = null;
        if (animator.IsInTransition(layer))
            clips = animator.GetNextAnimatorClipInfo(layer);
        if (clips == null || clips.Length == 0 || clips[0].clip == null)
            clips = animator.GetCurrentAnimatorClipInfo(layer);
        if (clips == null || clips.Length == 0 || clips[0].clip == null)
            return false;
        if (!IsSwingClipName(clips[0].clip.name))
            return false;

        clipName = clips[0].clip.name;
        length = clips[0].clip.length;
        return length > 0.01f;
    }

    public static bool IsSwingPlaying(Animator animator, float minNormalized)
    {
        if (animator == null)
            return false;

        if (animator.IsInTransition(0))
        {
            AnimatorClipInfo[] nextClips = animator.GetNextAnimatorClipInfo(0);
            if (nextClips != null && nextClips.Length > 0 && nextClips[0].clip != null &&
                IsSwingClipName(nextClips[0].clip.name))
            {
                float nextN = animator.GetNextAnimatorStateInfo(0).normalizedTime;
                return nextN >= 0f && nextN < minNormalized;
            }
        }

        AnimatorClipInfo[] clips = animator.GetCurrentAnimatorClipInfo(0);
        if (clips == null || clips.Length == 0 || clips[0].clip == null ||
            !IsSwingClipName(clips[0].clip.name))
            return false;

        float n = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        return n >= 0f && n < minNormalized;
    }

    static bool TryVariantFromPrefix(string prefix, out PlayerBasicAttackVariant variant)
    {
        switch (prefix)
        {
            case Prefix01:
                variant = PlayerBasicAttackVariant.Atk01;
                return true;
            case Prefix02:
                variant = PlayerBasicAttackVariant.Atk02;
                return true;
            case Prefix03:
                variant = PlayerBasicAttackVariant.Atk03;
                return true;
            default:
                variant = default;
                return false;
        }
    }
}
