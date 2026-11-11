using UnityEngine;

public partial class EntityActionVisual
{
    static void PlayUserLocomotion(UserEntity user, string move, float? fadeDuration = null, bool force = false)
    {
        if (user == null || user.IsDead())
            return;
        // During an attack swing we must NOT override the stance back to a stand
        // (wait/atkwait), but locomotion (run/walk) must still play so a chasing
        // pawn keeps running toward its target instead of freezing mid-swing.
        if (!force && user.IsAttackVisualPlaying() && move != null &&
            (move == "wait" || move == "atkwait"))
            return;

        NetworkAnimationController nac = user.GetAnimatorController();
        Animator animator = nac != null ? nac.GetAnimator() : null;
        string suffix = user.WeaponAnim;
        string state = move + "_" + suffix;
        // Debug.Log(user.LogTag + " Visual.PlayUserLocomotion move=" + move +
            // " weapon=" + suffix + " state=" + state +
            // " fade=" + (fadeDuration.HasValue ? fadeDuration.Value.ToString("F2") : "default") +
            // " nac=" + (nac != null) +
            // " animator=" + (animator != null ? animator.name : "null") +
            // " controller=" + (animator != null && animator.runtimeAnimatorController != null
                // ? animator.runtimeAnimatorController.name : "null"));
        if (nac == null)
            return;

        nac.SetAnimatorSpeed(1f);
        nac.SetPAtkSpeed(1f);
        float duration = fadeDuration ?? LocomotionCrossFadeSettings.ResolveDuration(state, nac);
        bool already = PlayerLocomotionCrossFade.IsAlreadyPlaying(nac, state);
        bool played;
        if (force)
        {
            // Bypass IsAlreadyPlaying short-circuit: caller explicitly wants this
            // state (re), even if the animator reports the clip as already active
            // or <none> mid-transition (pickup->run race).
            nac.CrossFadeInFixedTime(state, duration);
            played = true;
        }
        else
        {
            played = PlayerLocomotionCrossFade.TryPlay(nac, state, duration);
        }
        MagicGaitTrace.Event(user, "PlayUserLocomotion",
            "move=" + move +
            " state=" + state +
            " force=" + force +
            " already=" + already +
            " played=" + played +
            " fade=" + duration.ToString("F3"));
    }

    static void PlayUserMeleeAttack(UserEntity user)
    {
        if (user == null || user.Identity == null)
            return;

        NetworkAnimationController nacCheck = user.GetAnimatorController();
        Animator animator = nacCheck != null ? nacCheck.GetAnimator() : null;
        bool visLock = user.IsAttackVisualPlaying() && user.AttackVisualLeftSec() > 0.35f;
        bool clipLock = PlayerBasicAttackAnim.IsSwingPlaying(animator, 0.85f);
        if (visLock || clipLock)
        {
            // EntityActionCombatLog.LogCiPawn(user,
                // "Melee SKIP_RETRIGGER nick=" + EntityActionCombatLog.NameOf(user) +
                // " visLock=" + visLock +
                // " clipLock=" + clipLock +
                // " visLeft=" + user.AttackVisualLeftSec().ToString("F3") +
                // " " + EntityActionCombatLog.AnimDump(user));
            return;
        }

        string prefix = UserMeleeVariants[UnityEngine.Random.Range(0, UserMeleeVariants.Length)];
        string state = prefix + user.WeaponAnim;
        float cycleMs = AttackTimingHelper.ResolveAttackCycleMs(user, state);
        user.BeginAttackVisual(cycleMs / 1000f);
        NetworkAnimationController nacSpeed = user.GetAnimatorController();
        if (nacSpeed != null)
            nacSpeed.SetAnimatorSpeed(1f);

        if (ControllerPacketServices.Animations != null)
        {
            ControllerPacketServices.Animations.PlayAnimationTrigger(user.Identity.Id, prefix);
            // Debug.Log(user.LogTag + " Visual.MeleeAttack state=" + state + " cycleMs=" + cycleMs);
            return;
        }

        NetworkAnimationController nac = user.GetAnimatorController();
        if (nac == null)
            return;

        PlayerBasicAttackCrossFade.TryPlay(nac, state);
        // Debug.Log(user.LogTag + " Visual.MeleeAttack fallback state=" + state + " cycleMs=" + cycleMs);
    }
}
