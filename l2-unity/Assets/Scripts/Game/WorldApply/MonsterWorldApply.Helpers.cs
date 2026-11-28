using UnityEngine;

public sealed partial class MonsterWorldApply : EntityWorldApply
{
    public override void OnAttack(Entity attacker, Entity target, AttackDto dto)
    {
        if (attacker == null || attacker.IsDead() || target == null || attacker == target)
            return;

        if (CombatTrace.Enabled)
            CombatTrace.Write("anim.start",
                "who=npc atk=" + (attacker.Identity != null ? attacker.Identity.Id : 0) +
                " tgt=" + (target.Identity != null ? target.Identity.Id : 0) +
                " cycleMs=" + AttackTimingHelper.ResolveAttackCycleMs(attacker, null).ToString("F0") +
                " pAtkSpd=" + (attacker.Stats != null ? attacker.Stats.PAtkSpd : 0));
        _actions.Set(attacker, EntityActionKind.Attack, dto);
    }

    public override void OnMagicSkillUse(Entity entity, MagicSkillUseDto dto)
    {
        bool windstrike = dto != null && SkillPresentationCatalog.IsWindStrike(dto.SkillId);
        if (windstrike)
        {
            Debug.Log(
                "[WINDSTRIKE] Monster.OnMagicSkillUse dead=" + (entity != null && entity.IsDead()) +
                " actions=" + (_actions != null));
        }
        if (entity == null || entity.IsDead())
            return;
        if (EntityActionSkill.TryApplyWeaponCharge(entity, dto))
        {
            if (windstrike)
                Debug.Log("[WINDSTRIKE] Monster.OnMagicSkillUse stopped at weapon-charge");
            return;
        }
        if (windstrike)
            Debug.Log("[WINDSTRIKE] Monster.OnMagicSkillUse Set Skill");
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

    public override void OnMagicSkillCanceled(Entity entity, MagicSkillCanceledDto dto)
    {
        if (entity != null && entity.IsDead())
            return;
        if (entity != null && entity.Identity != null)
            CombatPacketServices.Facing?.EndFollow(entity.Identity.Id, "skill-canceled");
        _actions.Set(entity, EntityActionKind.Idle, null);
    }
}
