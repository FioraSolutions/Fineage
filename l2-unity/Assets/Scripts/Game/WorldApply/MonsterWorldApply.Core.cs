using UnityEngine;

public sealed partial class MonsterWorldApply : EntityWorldApply
{
    public MonsterWorldApply(EntityActionMachine actions)
    {
        _actions = actions;
    }

    public override void OnMoveTo(Entity entity, Vector3 destination, Vector3 current, CharMoveToLocationDto dto)
    {
        if (entity == null || entity.IsDead())
            return;
        // EntityActionCombatLog.LogIfWatch(entity,
            // "Monster.MoveTo chase action=" + entity.ActionSlot.Action +
            // " inCombat=" + entity.InCombat);
        DebugLineDraw.ShowDrawLineDebug(dto.ObjId, destination, current, Color.red);
        _actions.Set(entity, EntityActionKind.Move, dto);
    }

    public override void OnStopMove(Entity entity, StopMoveDto dto)
    {
        _actions.ApplyStop(entity, dto);
    }

    public override void OnMoveToPawn(Entity entity, MoveToPawnDto dto)
    {
        if (entity == null || entity.IsDead())
            return;
        // EntityActionCombatLog.LogIfWatch(entity,
            // "Monster.MoveToPawn chase action=" + entity.ActionSlot.Action +
            // " inCombat=" + entity.InCombat);
        _actions.Set(entity, EntityActionKind.Move, dto);
    }

    public override void OnDie(Entity entity, DieDto dto)
    {
        _actions.Die(entity, false, "packet");

        PlayerEntity player = DomainPacketServices.LocalPlayer;
        bool targetMatch = player != null && dto != null && player.TargetId == dto.ObjectId;
        if (targetMatch)
            player.IsAttack = false;

        bool localInCombat = LocalPlayerInCombatWith(player, targetMatch);
        // WaitReturnLog.Dump(
            // "MonsterWorldApply.OnDie targetMatch=" + targetMatch +
            // " localInCombat=" + localInCombat +
            // " skipWaitReturn=" + !localInCombat,
            // entity,
            // dto);
        if (localInCombat && PlayerStatePacketServices.PlayerState != null)
            PlayerStatePacketServices.PlayerState.OnWaitReturn();
    }

    static bool LocalPlayerInCombatWith(PlayerEntity player, bool targetMatch)
    {
        if (player == null || !targetMatch)
            return false;
        if (player.isAutoAttack || player.IsAttack)
            return true;
        PlayerStateMachine sm = PlayerStatePacketServices.PlayerState;
        if (sm == null)
            return false;
        PlayerState state = sm.State;
        return state == PlayerState.ATTACKING ||
            state == PlayerState.PHYSICAL_SKILLS ||
            state == PlayerState.MAGIC_SKILLS ||
            state == PlayerState.ANIMATION_LOCKED;
    }
}
