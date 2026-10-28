using UnityEngine;

/// <summary>
/// Local player packet apply. No VContainer ctor deps — hero spawns after GameScope build.
/// </summary>
public sealed class PlayerWorldApply : EntityWorldApply
{
    private SkillbarWindow Skillbar =>
        IncomingPacketActions.Skillbar != null
            ? IncomingPacketActions.Skillbar
            : SkillbarWindow.Instance;

    private EffectManager Effects =>
        IncomingPacketActions.Effects != null
            ? IncomingPacketActions.Effects
            : EffectManager.Instance;

    private PlayerController Player => IncomingPacketActions.Player;

    private EventBus Bus =>
        IncomingPacketActions.Bus != null ? IncomingPacketActions.Bus : EventBus.Instance;

    public override void OnMoveTo(Entity entity, Vector3 destination, Vector3 current, CharMoveToLocationDto dto)
    {
        if (entity != null && entity.IsDead())
            return;

        if (PlayerStateMachine.Instance != null)
            PlayerStateMachine.Instance.ChangeIntention(Intention.INTENTION_MOVE_TO, dto);
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

        if (WeaponChargeSkill.IsCharge(dto.SkillId))
        {
            ApplyWeaponChargeShot(entity, dto);
            return;
        }

        if (SetupDurationHelper.IsUsePotion(dto))
        {
            SetupDurationHelper.PlayPotionEffect(entity, dto);
            return;
        }

        if (PlayerStateMachine.Instance == null)
            return;

        PlayerStateMachine.Instance.ChangeIntention(
            dto.SkillGrp.IsMagic == 1 ?
                Intention.INTENTION_MAGIC_ATTACK :
                Intention.INTENTION_PHYSICAL_SKILLS_ATTACK,
            dto);
    }

    private void ApplyWeaponChargeShot(Entity entity, MagicSkillUseDto useSkill)
    {
        PlayerEntity player = entity as PlayerEntity;
        if (player == null || useSkill == null)
            return;

        player.IsSoulshotCharged = true;
        EffectManager effects = Effects;
        if (effects == null)
            return;
        WeaponChargeSkill.Play(effects, useSkill.SkillId, player.GetWeaponTransform(), player.transform);
    }

    public override void OnAutoAttackStart(Entity entity, AutoAttackStartDto dto)
    {
        PlayerEntity player = entity as PlayerEntity;
        if (player == null || player.IsDead())
            return;
        player.isAutoAttack = true;
        RefreshPlayerStandWait(player);
    }

    public override void OnAutoAttackStop(Entity entity, AutoAttackStopDto dto)
    {
        PlayerEntity player = entity as PlayerEntity;
        if (player == null)
            return;
        player.isAutoAttack = false;
        RefreshPlayerStandWait(player);
    }

    void RefreshPlayerStandWait(PlayerEntity player)
    {
        if (player == null || player.IsDead() || player.IsAttack)
            return;
        if (PlayerStateMachine.Instance == null)
            return;

        PlayerState state = PlayerStateMachine.Instance.State;
        if (state == PlayerState.ATTACKING ||
            state == PlayerState.PHYSICAL_SKILLS ||
            state == PlayerState.MAGIC_SKILLS ||
            state == PlayerState.ANIMATION_LOCKED ||
            state == PlayerState.RUNNING ||
            state == PlayerState.WALKING ||
            state == PlayerState.SITTING ||
            state == PlayerState.SIT_WAIT ||
            state == PlayerState.REBIRTH ||
            state == PlayerState.DEAD)
            return;
        PlayerController controller = Player;
        if (controller != null && controller.RunningToDestination)
            return;

        PlayerStateMachine.Instance.NotifyEvent(Event.ARRIVED);
    }

    public override void OnSocialAction(Entity entity, SocialActionDto dto)
    {
        EventBus bus = Bus;
        if (dto != null && dto.ActionId == 15 && bus != null)
            bus.LevelUp(entity, dto.ObjectId);
    }

    public override void OnMagicSkillCanceled(Entity entity, MagicSkillCanceledDto dto)
    {
        if (PlayerStateMachine.Instance != null)
            PlayerStateMachine.Instance.NotifyEvent(Event.CANCEL);
    }
}
