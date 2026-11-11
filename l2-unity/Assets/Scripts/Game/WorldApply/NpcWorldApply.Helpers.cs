using UnityEngine;

public sealed partial class NpcWorldApply : EntityWorldApply
{
    public override void OnMagicSkillUse(Entity entity, MagicSkillUseDto dto)
    {
        if (entity == null || entity.IsDead())
            return;
        if (EntityActionSkill.TryApplyWeaponCharge(entity, dto))
            return;
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

    public override void OnNpcHtml(Entity entity, NpcHtmlMessageDto dto)
    {
        PlayerEntity player = DomainPacketServices.LocalPlayer;
        if (player != null && entity != null)
        {
            Vector3 direction = player.transform.position - entity.transform.position;
            entity.transform.rotation = Quaternion.LookRotation(direction);
        }

        if (_html == null || dto == null)
            return;

        _html.InjectToWindow(dto.Html);
        _html.ShowWindowToCenter();
    }
}
