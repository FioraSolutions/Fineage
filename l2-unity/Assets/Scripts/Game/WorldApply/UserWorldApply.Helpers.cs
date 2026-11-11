
using UnityEngine;

public sealed partial class UserWorldApply : EntityWorldApply
{
    public override void OnAttack(Entity attacker, Entity target, AttackDto dto)
    {
        if (attacker == null || attacker.IsDead() || attacker == target)
            return;
        _actions.Set(attacker, EntityActionKind.Attack, dto);
    }

    public override void OnMagicSkillUse(Entity entity, MagicSkillUseDto dto)
    {
        if (entity == null || entity.IsDead())
            return;
        if (EntityActionSkill.TryApplyWeaponCharge(entity, dto))
            return;
        // EntityActionCombatLog.LogCiPawn(entity,
            // "MagicSkillUse APPLY CharInfo nick=" + EntityActionCombatLog.NameOf(entity) +
            // " skill=" + (dto != null ? dto.SkillId.ToString() : "-") +
            // " lvl=" + (dto != null ? dto.SkillLvl.ToString() : "-") +
            // " hitTime=" + (dto != null ? dto.HitTime.ToString() : "-"));
        _actions.Set(entity, EntityActionKind.Skill, dto);
    }

    public override void OnAutoAttackStart(Entity entity, AutoAttackStartDto dto)
    {
        _actions.StartAttackStance(entity);
    }

    public override void OnAutoAttackStop(Entity entity, AutoAttackStopDto dto)
    {
        _actions.StopAttack(entity);
    }

    public override void OnSocialAction(Entity entity, SocialActionDto dto)
    {
        _actions.Social(entity, dto);
    }

    public override void OnChangeWaitType(Entity entity, ChangeWaitTypeDto dto)
    {
        _actions.ChangeWaitType(entity, dto);
    }

    public override void OnMagicSkillCanceled(Entity entity, MagicSkillCanceledDto dto)
    {
        if (entity != null && entity.IsDead())
            return;
        if (entity != null && entity.Identity != null)
            CombatPacketServices.Facing?.EndFollow(entity.Identity.Id, "skill-canceled");
        _actions.Set(entity, EntityActionKind.Idle, null);
    }
}
