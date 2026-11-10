using UnityEngine;

public static partial class EntityActionVisual
{
    public static void PlayDeath(Entity entity, bool alreadyCorpse = false)
    {
        if (entity == null)
            return;

        entity.SetDead(true);
        if (!CanPlayAnim(entity))
            return;

        if (entity is UserEntity user)
        {
            if (!alreadyCorpse && IsPlayingDeath(entity))
                return;
            PlayUserDeath(user, alreadyCorpse);
            return;
        }

        if (entity is not MonsterEntity monster || ControllerPacketServices.Animations == null)
            return;

        // Shared Monster_* path for every mob. Death is terminal: if the clip or held
        // corpse pose is already up (client predict + Die, or pool leftover cleared on spawn),
        // do not CrossFade again — MonsterCrossFade also skips when already in "death".
        if (IsMonsterDeathVisualActive(monster))
            return;

        // Combat can leave animator.speed / patkspd at 0; death SMB needs a live clock.
        NetworkAnimationController nac = monster.GetAnimatorController();
        if (nac != null)
        {
            nac.SetPAtkSpeed(1f);
            nac.SetAnimatorSpeed(1f);
        }

        if (!alreadyCorpse)
            MonsterCombatSound.Play(MonsterSoundEvent.Death, monster);

        ControllerPacketServices.Animations.PlayMonsterAnimation(
            monster.Identity.Id, AnimationNames.DEAD.ToString());
    }

    static bool IsMonsterDeathVisualActive(MonsterEntity monster)
    {
        if (monster == null)
            return false;
        if (monster.ActionSlot != null && monster.ActionSlot.DeathPoseHeld)
            return true;
        return IsPlayingDeath(monster);
    }

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

    static readonly string[] UserMeleeVariants = { "jatk01_", "jatk02_", "jatk03_" };

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

    static void PlayUserPhysicalSkill(UserEntity user, MagicSkillUseDto magic)
    {
        if (user == null || user.Identity == null)
            return;

        string trigger = ResolveUserPhysicalSkillTrigger(magic);
        bool basicJatk = !string.IsNullOrEmpty(trigger) &&
            trigger.StartsWith("jatk", System.StringComparison.OrdinalIgnoreCase);
        NetworkAnimationController nac = user.GetAnimatorController();
        Animator animator = nac != null ? nac.GetAnimator() : null;
        if (basicJatk &&
            (user.IsAttackVisualPlaying() || PlayerBasicAttackAnim.IsSwingPlaying(animator, 0.85f)))
        {
            // EntityActionCombatLog.LogCiPawn(user,
                // "Skill SKIP_JATK nick=" + EntityActionCombatLog.NameOf(user) +
                // " skill=" + (magic != null ? magic.SkillId.ToString() : "-") +
                // " lvl=" + (magic != null ? magic.SkillLvl.ToString() : "-") +
                // " trigger=" + trigger +
                // " visLeft=" + user.AttackVisualLeftSec().ToString("F3") +
                // " " + EntityActionCombatLog.AnimDump(user));
            return;
        }

        int objectId = user.Identity.Id;
        if (ControllerPacketServices.Animations != null && magic != null && magic.HitTime > 0)
            ControllerPacketServices.Animations.SetSpTimeAtk(objectId, magic.HitTime);

        string state = trigger + user.WeaponAnim;

        float cycleMs = magic != null && magic.HitTime > 0
            ? magic.HitTime
            : AttackTimingHelper.ResolveAttackCycleMs(user, state);
        user.BeginAttackVisual(cycleMs / 1000f);

        if (ControllerPacketServices.Animations != null)
        {
            ControllerPacketServices.Animations.PlayAnimationTrigger(objectId, trigger);
            // Debug.Log(user.LogTag + " Visual.PhysicalSkill trigger=" + trigger +
                // " state=" + state + " cycleMs=" + cycleMs);
            return;
        }

        PlayUserMeleeAttack(user);
    }

    static string ResolveUserPhysicalSkillTrigger(MagicSkillUseDto magic)
    {
        if (magic == null || TablePacketServices.SkillGrps == null)
            return UserMeleeVariants[UnityEngine.Random.Range(0, UserMeleeVariants.Length)];

        AnimationCombo combo = TablePacketServices.SkillGrps.GetAnimComboBySkillId(magic.SkillId, magic.SkillLvl);
        if (combo == null)
            return UserMeleeVariants[UnityEngine.Random.Range(0, UserMeleeVariants.Length)];

        string[] cycle = combo.GetAnimCycle();
        if (cycle == null)
            return UserMeleeVariants[UnityEngine.Random.Range(0, UserMeleeVariants.Length)];

        for (int i = 0; i < cycle.Length; i++)
        {
            string name = cycle[i];
            if (string.IsNullOrWhiteSpace(name))
                continue;

            string trimmed = name.Trim();
            if (string.Equals(trimmed, "none", System.StringComparison.OrdinalIgnoreCase))
                continue;
            if (trimmed.StartsWith("Cast", System.StringComparison.Ordinal) ||
                trimmed.StartsWith("MagicShot", System.StringComparison.Ordinal))
                break;
            if (trimmed.StartsWith("SpAtk", System.StringComparison.OrdinalIgnoreCase))
                return trimmed.EndsWith("_") ? trimmed : trimmed + "_";
        }

        return UserMeleeVariants[UnityEngine.Random.Range(0, UserMeleeVariants.Length)];
    }

    static void PlayUserDeath(UserEntity user, bool alreadyCorpse)
    {
        if (user == null || user.Identity == null || ControllerPacketServices.Animations == null)
            return;

        user.ClearAttackVisual();
        ControllerPacketServices.Animations.PlayExactAnimatorState(
            user.Identity.Id, PlayerDeathAnim.Death, alreadyCorpse);
        // Debug.Log(user.LogTag + " Visual.Death alreadyCorpse=" + alreadyCorpse);
    }

    public static void PlayRevive(Entity entity)
    {
        UserEntity user = entity as UserEntity;
        if (user == null || user.Identity == null || ControllerPacketServices.Animations == null)
            return;

        user.SetDead(false);
        user.ClearAttackVisual();
        float duration = ControllerPacketServices.Animations.PlayExactAnimatorState(
            user.Identity.Id, PlayerDeathAnim.Rebirth);
        user.BeginAttackVisual(duration);
        // Debug.Log(user.LogTag + " Visual.Revive duration=" + duration);
    }
}
