using System;

using UnityEngine;

public partial class PlayerController
{
    public void MoveToPoint(MovementTarget movementTarget)
    {
        bool wasMoving = _isMove;
        ActivateIfNotMoving(_isMove);
        if (!wasMoving && !KeepsRunGait())
            _moveTimer = 0f;
        _movementTarget = movementTarget;
        _isMove = true;
    }

    public void StopMove()
    {
        _isMove = false;
        if (_packetClicks != null)
            _packetClicks.HideLocator();
        _moveStoppedAt = Time.time;
        _switchWalkToRun = false;

    }

    private void ActivateIfNotMoving(bool is_move)
    {
         if (!is_move)
         {
            _switchWalkToRun = false;
            _isFirst = true;
         }
    }

    public void ThinkMoveToPawn( ModelMovePawn model)
    {
        //PlayerStatePacketServices.PlayerState.IsMoveToPawn = true;
        
        PlayerStatePacketServices.PlayerState.ChangeIntention(Intention.INTENTION_MOVE_TO_PAWN, model);
    }

    public void InitMoveToPawn(MoveToPawnDto moveToPawnPacket)
    {
        if (DomainPacketServices.LocalPlayer == null)
            return;

        // Engine OnMoveToPawn uses the packet's pawn directly. It does not require
        // that pawn to be the UI-selected target. During replay the first attacked
        // goblin is still selected while this packet already follows the second one.
        // Same registry World.GetEntityNoLockSync reads. This path does not take World.
        Entity target = Spawned.Find(moveToPawnPacket.TarObjid);
        if (target == null)
        {
            // ReplayAttackTrace.Write(
                // "MOVE_TO_PAWN_SKIP",
                // DomainPacketServices.LocalPlayer,
                // null,
                // "targetId=" + moveToPawnPacket.TarObjid);
            return;
        }

        // ReplayAttackTrace.Write(
            // "MOVE_TO_PAWN",
            // DomainPacketServices.LocalPlayer,
            // target,
            // "distanceUu=" + moveToPawnPacket.DistanceUu +
            // " distanceMeters=" + moveToPawnPacket.Distance.ToString("F3") +
            // " selectedTargetId=" + DomainPacketServices.LocalPlayer.TargetId);
        ThinkMoveToPawn(new ModelMovePawn(DomainPacketServices.LocalPlayer, target, moveToPawnPacket.Distance));
    }

    private float ApplyGravity(float deltaTime)
    {
        // Hole in the walkable mesh: geodata (or the current deck) still says we
        // should be here, but the next collider is a distant shell. Do not fall
        // through. A real cliff has geodata far below, so gravity still runs.
        bool meshHole = !Placement.HasFloorWithin(transform.position, WorldGrounding.SnapToleranceMeters);
        bool stayOnDeck = false;
        if (meshHole)
        {
            if (Placement.TryResolveGeodataY(transform.position, out float geoY)
                && Mathf.Abs(geoY - transform.position.y) <= WorldGrounding.SnapToleranceMeters)
                stayOnDeck = true;
        }
        if (_controller.isGrounded)
        {
            _verticalVelocity = -2f;
        }
        else if (stayOnDeck)
        {
            _verticalVelocity = 0f;
        }
        else
        {
            _verticalVelocity -= _gravity * deltaTime;
            if (_verticalVelocity < -TerminalVelocity)
                _verticalVelocity = -TerminalVelocity;
        }
        return _verticalVelocity;
    }

    void RecoverFeetIfUnderFloor()
    {
        if (!Placement.TryLiftOutOfFloor(transform.position, out float deck))
            return;
        Vector3 p = transform.position;
        p.y = deck;
        Placement.ApplyPose(gameObject, p, transform.rotation);
        _verticalVelocity = -2f;
    }

}
