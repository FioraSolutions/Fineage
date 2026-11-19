using System;
using System.Collections.Generic;
using UnityEngine;

public partial class EntityActionSkill
{
    static bool IsSkillAnimationNone(MagicSkillUseDto magic)
    {
        if (magic == null)
            return true;
        if (magic.SkillGrp == null)
            return true;
        string anim = magic.SkillGrp.Animation;
        return string.IsNullOrWhiteSpace(anim) ||
               string.Equals(anim, "None", StringComparison.OrdinalIgnoreCase);
    }

    static AnimationCombo GetCombo(MagicSkillUseDto magic)
    {
        if (magic == null || TablePacketServices.SkillGrps == null)
            return null;
        return TablePacketServices.SkillGrps.GetAnimComboBySkillId(magic.SkillId, magic.SkillLvl);
    }

    static AnimationEventsBase ResolveEvents(Entity entity)
    {
        if (entity == null || entity.Identity == null || ControllerPacketServices.Animations == null)
            return null;
        return ControllerPacketServices.Animations.GetAnimationEvents(entity.Identity.Id);
    }

    /// <summary>
    /// True when the skill must run through the cast graph. <c>is_magic=1</c> is the mage flag, but the
    /// Interlude client really keys the animation off the skillgrp <c>animation</c> tag: Trick (11) and
    /// Switch (12) are <c>is_magic=0</c> dagger utilities with <c>{[C]}</c> = CastShort → CastEnd →
    /// MagicThrow, and Spoil (254) / Unlock (27) sit on full cast tags too. Those tags are only patches
    /// over the cast graph — CastShort maps to CastMid, MagicThrow to MagicShot — so they must take the
    /// override runner (cast bar, stationary cast, shoot notify, split target art). The physical path
    /// asked StartRun for a "CastShort_" / "MagicThrow_" state that does not exist: the hero stood still,
    /// the packet timing ran out with no art at all, and no cast bar appeared.
    /// </summary>
    static bool IsMagicSkill(MagicSkillUseDto magic)
    {
        if (magic == null || magic.SkillGrp == null)
            return false;
        return magic.SkillGrp.IsMagic == 1 || SetupDurationHelper.UsesCastAnimation(magic);
    }

    /// <summary>
    /// Mobius <c>Skill.isBad()</c> — <c>effectPoint &lt; 0 &amp;&amp; targetType != SELF</c>. Creature.onMagicFinalizer
    /// raises the attack stance (<c>clientStartAutoAttack</c> → <c>AutoAttackStart</c> → <c>atkwait</c>) only for a bad
    /// skill, so a self/party buff must not flip the actor into the combat pose.
    /// <para>
    /// skillgrp carries no target type, so SELF is read from the packet: <paramref name="selfCast"/> is true when the
    /// caster is the target, which is what the server sends for Dash (4), Might, Wind Walk and the heals. A bad skill
    /// always targets another object. Potions (skill handler families) are never bad either.
    /// </para>
    /// <para>
    /// Only ever turns the stance on. A cast that is not bad leaves the existing 15 s
    /// <c>AttackStanceTaskManager</c> window alone — the same way Mobius does not send AutoAttackStop on a buff.
    /// </para>
    /// </summary>
    static bool SkillPutsActorInAttackStance(MagicSkillUseDto magic, bool selfCast)
    {
        if (magic == null || selfCast || SetupDurationHelper.IsUsePotion(magic))
            return false;
        Skillgrp grp = magic.SkillGrp;
        return grp != null && grp.EffectPoint < 0;
    }
}
