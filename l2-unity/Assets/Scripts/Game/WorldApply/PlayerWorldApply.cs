using UnityEngine;

/// <summary>
/// Local player packet apply. No VContainer ctor deps — hero spawns after GameScope build.
/// </summary>
public sealed class PlayerWorldApply : EntityWorldApply
{
    private SkillbarWindow Skillbar =>
        SkillbarPacketServices.Skillbar ?? SkillbarPacketServices.Skillbar;

    private EffectManager Effects => GlobalEffectsPacketServices.Effects;

    private PlayerController Player =>
        WorldApplyPacketServices.Player ?? ControllerPacketServices.Player;

    private EventBus Bus =>
        WorldApplyPacketServices.Bus ?? GamePacketServices.Bus;

    public override void OnMoveTo(Entity entity, Vector3 destination, Vector3 current, CharMoveToLocationDto dto)
    {
        if (entity != null && entity.IsDead())
            return;

        if (PlayerStatePacketServices.PlayerState != null)
            PlayerStatePacketServices.PlayerState.ChangeIntention(Intention.INTENTION_MOVE_TO, dto);
    }

    public override void OnPositionCorrection(Entity entity, Vector3 position)
    {
        SnapTransform(entity, position);
        // Drop any local MoveTo that was racing ValidatePosition rubber-bands.
        LocalCastStop.Halt();
    }

    public override void OnStopMove(Entity entity, StopMoveDto dto)
    {
        if (PlayerStatePacketServices.PlayerState == null || PlayerStatePacketServices.PlayerState.State == PlayerState.DEAD)
            return;

        if (!entity.GetDead())
            PlayerStatePacketServices.PlayerState.ChangeIntention(Intention.INTENTION_STOP_MOVE, dto);
    }

    public override void OnMoveToPawn(Entity entity, MoveToPawnDto dto)
    {
        if (entity != null && entity.IsDead())
            return;

        PlayerController controller = entity.GetComponent<PlayerController>();
        if (controller != null)
            controller.InitMoveToPawn(dto);
    }

    public override void OnDie(Entity entity, DieDto dto)
    {
        entity.SetDead(true);
        if (EntityActionPacketServices.Actions != null)
            EntityActionPacketServices.Actions.Die(entity);
        if (PlayerStatePacketServices.PlayerState != null)
            PlayerStatePacketServices.PlayerState.ChangeIntention(Intention.INTENTION_DEAD, dto);
    }

    public override void OnRevive(Entity entity, ReviveDto dto)
    {
        entity.SetDead(false);
        DeadWindow dead = ControllerPacketServices.Dead;
        if (dead != null)
            dead.HideWindow();
        if (PlayerStatePacketServices.PlayerState != null)
        {
            PlayerStatePacketServices.PlayerState.ChangeState(PlayerState.REBIRTH);
            PlayerStatePacketServices.PlayerState.NotifyEvent(Event.REBIRTH);
        }
    }

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

    /// <summary>
    /// When standing idle, re-fire WAIT_RETURN so NewIdleState CrossFades wait vs atkwait
    /// from the updated <see cref="PlayerEntity.isAutoAttack"/> flag (NPC parity via
    /// EntityActionMachine.RefreshStandWaitIfIdle). Skips cast/swing/locomotion.
    /// </summary>
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
