using UnityEngine;

public partial class EntityActionVisual
{
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
