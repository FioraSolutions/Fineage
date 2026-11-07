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

        if (PlayerStateMachine.Instance != null)
            PlayerStateMachine.Instance.ChangeIntention(Intention.INTENTION_MOVE_TO, dto);
    }

    public override void OnPositionCorrection(Entity entity, Vector3 position)
    {
        SnapTransform(entity, position);
        // Drop any local MoveTo that was racing ValidatePosition rubber-bands.
        LocalCastStop.Halt();
    }

    public override void OnStopMove(Entity entity, StopMoveDto dto)
    {
        if (PlayerStateMachine.Instance == null || PlayerStateMachine.Instance.State == PlayerState.DEAD)
            return;

        if (!entity.GetDead())
            PlayerStateMachine.Instance.ChangeIntention(Intention.INTENTION_STOP_MOVE, dto);
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
        if (EntityActionMachine.Instance != null)
            EntityActionMachine.Instance.Die(entity);
        if (PlayerStateMachine.Instance != null)
            PlayerStateMachine.Instance.ChangeIntention(Intention.INTENTION_DEAD, dto);
    }

    public override void OnRevive(Entity entity, ReviveDto dto)
    {
        entity.SetDead(false);
        DeadWindow dead = ControllerPacketServices.Dead;
        if (dead != null)
            dead.HideWindow();
        if (PlayerStateMachine.Instance != null)
        {
            PlayerStateMachine.Instance.ChangeState(PlayerState.REBIRTH);
            PlayerStateMachine.Instance.NotifyEvent(Event.REBIRTH);
        }
    }

    public override void OnAttack(Entity attacker, Entity target, AttackDto dto)
    {
        if (attacker == null || target == null || attacker == target)
            return;

        if (attacker.IsDead())
            return;

        if (PlayerStateMachine.Instance != null)
            PlayerStateMachine.Instance.ChangeIntention(Intention.INTENTION_ATTACK, dto);
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
        // Mobius AttackStanceTaskManager: stance flag only — idle pose reads isAutoAttack.
        PlayerEntity player = entity as PlayerEntity;
        if (player == null || player.IsDead())
            return;
        player.isAutoAttack = true;
    }

    public override void OnAutoAttackStop(Entity entity, AutoAttackStopDto dto)
    {
        PlayerEntity player = entity as PlayerEntity;
        if (player == null)
            return;
        player.isAutoAttack = false;
    }

    public override void OnSocialAction(Entity entity, SocialActionDto dto)
    {
        // PlaySocial fires EventBus.LevelUp for action 15 + social01 gesture.
        if (EntityActionMachine.Instance != null)
            EntityActionMachine.Instance.Social(entity, dto);
        else
            EntityActionVisual.PlaySocial(entity, dto != null ? dto.ActionId : 0);
    }

    public override void OnMagicSkillCanceled(Entity entity, MagicSkillCanceledDto dto)
    {
        MagicSkillPresentation.EndCastChannel();
        if (PlayerStateMachine.Instance != null)
            PlayerStateMachine.Instance.NotifyEvent(Event.CANCEL);
    }
}
