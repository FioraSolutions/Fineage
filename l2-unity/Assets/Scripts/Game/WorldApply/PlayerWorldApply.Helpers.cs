using UnityEngine;

public sealed partial class PlayerWorldApply : EntityWorldApply
{
    public override void OnAttack(Entity attacker, Entity target, AttackDto dto)
    {
        if (attacker == null || target == null || attacker == target)
            return;

        if (attacker.IsDead())
            return;

        if (PlayerStatePacketServices.PlayerState != null)
            PlayerStatePacketServices.PlayerState.ChangeIntention(Intention.INTENTION_ATTACK, dto);
    }

    public override void OnMagicSkillUse(Entity entity, MagicSkillUseDto dto)
    {
        SkillbarWindow skillbar = Skillbar;
        if (skillbar != null)
            skillbar.ShowCooldown(dto.SkillId, Shortcut.TYPE_SKILL, dto.Reusedelay);
        if (SkillLearnPacketServices.SkillList != null)
            SkillLearnPacketServices.SkillList.ShowCooldown(dto.SkillId, dto.Reusedelay);

        // Cooldown UI only here. Stop/SFX/bar/intention: MagicSkillPresentation (Mobius beginCast).
        MagicSkillPresentation.PresentLocal(entity, dto);
    }

    public override void OnAutoAttackStart(Entity entity, AutoAttackStartDto dto)
    {
        // Mobius AttackStanceTaskManager: stance flag + refresh wait↔atkwait when already idle.
        // Do not ChangeIntention / CrossFade during cast or swing (see magic-skill-use-flow).
        PlayerEntity player = entity as PlayerEntity;
        if (player == null || player.IsDead())
            return;
        player.isAutoAttack = true;
        RefreshLocalStandWaitIfIdle();
    }

    public override void OnAutoAttackStop(Entity entity, AutoAttackStopDto dto)
    {
        PlayerEntity player = entity as PlayerEntity;
        if (player == null)
            return;
        player.isAutoAttack = false;
        RefreshLocalStandWaitIfIdle();
    }

    static void RefreshLocalStandWaitIfIdle()
    {
        PlayerStateMachine sm = PlayerStatePacketServices.PlayerState;
        if (sm == null)
            return;
        if (sm.State != PlayerState.IDLE || sm.Intention != Intention.INTENTION_IDLE)
            return;
        if (sm.ShouldKeepLocomotion())
            return;
        PlayerEntity local = DomainPacketServices.LocalPlayer;
        if (local != null && local.IsAttack)
            return;
        sm.NotifyEvent(Event.WAIT_RETURN);
    }

    public override void OnSocialAction(Entity entity, SocialActionDto dto)
    {
        // PlaySocial fires EventBus.LevelUp for action 15 + social01 gesture.
        if (EntityActionPacketServices.Actions != null)
            EntityActionPacketServices.Actions.Social(entity, dto);
        else
            EntityActionVisual.PlaySocial(entity, dto != null ? dto.ActionId : 0);
    }

    public override void OnMagicSkillCanceled(Entity entity, MagicSkillCanceledDto dto)
    {
        MagicSkillPresentation.EndCastChannel();
        if (PlayerStatePacketServices.PlayerState != null)
            PlayerStatePacketServices.PlayerState.NotifyEvent(Event.CANCEL);
    }
}
