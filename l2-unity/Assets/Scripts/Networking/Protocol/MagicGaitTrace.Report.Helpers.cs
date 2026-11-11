using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

public static partial class MagicGaitTrace
{
    static void AppendAnimatorDump(StringBuilder sb, UserEntity user, int id)
    {
        if (user == null)
            return;
        NetworkAnimationController nac = user.GetAnimatorController();
        Animator anm = nac != null ? nac.GetAnimator() : null;
        if (anm == null)
        {
            sb.Append(" | anim=null");
            return;
        }

        string want = "run_" + user.WeaponAnim;
        AnimatorStateInfo cur = anm.GetCurrentAnimatorStateInfo(0);
        AnimatorClipInfo[] clips = anm.GetCurrentAnimatorClipInfo(0);
        string clip = clips != null && clips.Length > 0 && clips[0].clip != null
            ? clips[0].clip.name
            : "<none>";
        float norm = cur.normalizedTime;
        float normD = 0f;
        float prev;
        if (PrevNorm.TryGetValue(id, out prev))
            normD = norm - prev;
        PrevNorm[id] = norm;

        bool inTrans = anm.IsInTransition(0);
        string nextClip = "-";
        if (inTrans)
        {
            AnimatorClipInfo[] next = anm.GetNextAnimatorClipInfo(0);
            if (next != null && next.Length > 0 && next[0].clip != null)
                nextClip = next[0].clip.name;
        }

        bool already = PlayerLocomotionCrossFade.IsAlreadyPlaying(anm, want);
        bool hasState = anm.HasState(0, Animator.StringToHash(want));
        var ctrl = anm.runtimeAnimatorController;
        string ctrlName = ctrl != null ? ctrl.name : "null";

        bool castActive = false;
        BaseAnimationController bac = nac as BaseAnimationController;
        if (bac != null)
            castActive = bac.IsCastClipActive;

        sb.Append(" | want=").Append(want)
            .Append(" hasState=").Append(hasState)
            .Append(" already=").Append(already)
            .Append(" inTrans=").Append(inTrans)
            .Append(" clip=").Append(clip)
            .Append(" nextClip=").Append(nextClip)
            .Append(" normT=").Append(norm.ToString("F3"))
            .Append(" normD=").Append(normD.ToString("F3"))
            .Append(" animSpeed=").Append(anm.speed.ToString("F2"))
            .Append(" runSpeed=").Append(
                (AnimatorBoolSet.HasFloat(anm, "run_speed") ? anm.GetFloat("run_speed") : 0f).ToString("F3"))
            .Append(" walkSpeed=").Append(
                (AnimatorBoolSet.HasFloat(anm, "walk_speed") ? anm.GetFloat("walk_speed") : 0f).ToString("F3"))
            .Append(" castClip=").Append(castActive)
            .Append(" cull=").Append(anm.cullingMode)
            .Append(" enabled=").Append(anm.enabled)
            .Append(" ctrl=").Append(ctrlName);
    }

    static void AppendShortStack(StringBuilder sb, string full)
    {
        if (string.IsNullOrEmpty(full))
            return;
        int kept = 0;
        int start = 0;
        while (kept < 5 && start < full.Length)
        {
            int nl = full.IndexOf('\n', start);
            if (nl < 0)
                nl = full.Length;
            string line = full.Substring(start, nl - start).Trim();
            start = nl + 1;
            if (line.Length == 0)
                continue;
            if (line.IndexOf("MagicGaitTrace", StringComparison.Ordinal) >= 0)
                continue;
            if (line.IndexOf("Assets", StringComparison.Ordinal) < 0 &&
                line.IndexOf("EntityAction", StringComparison.Ordinal) < 0 &&
                line.IndexOf("MovementData", StringComparison.Ordinal) < 0 &&
                line.IndexOf("MoveAll", StringComparison.Ordinal) < 0 &&
                line.IndexOf("WorldApply", StringComparison.Ordinal) < 0 &&
                line.IndexOf("CastClip", StringComparison.Ordinal) < 0 &&
                line.IndexOf("AnimationManager", StringComparison.Ordinal) < 0 &&
                line.IndexOf("Skill", StringComparison.Ordinal) < 0 &&
                line.IndexOf("PlayerOverridden", StringComparison.Ordinal) < 0)
                continue;
            if (kept > 0)
                sb.Append(" < ");
            int at = line.IndexOf("at ", StringComparison.Ordinal);
            sb.Append(at >= 0 ? line.Substring(at + 3) : line);
            kept++;
        }
    }

    public static void Reset()
    {
        LastSyncAt.Clear();
        PrevNorm.Clear();
        WatchUntil.Clear();
    }
}
