using UnityEngine;

public sealed partial class PlayerWorldApply : EntityWorldApply
{
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
}
