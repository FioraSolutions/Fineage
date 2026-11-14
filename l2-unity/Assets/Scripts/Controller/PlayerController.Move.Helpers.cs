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
        _behindPlayer = VectorUtils.IsTargetBehindPlayer(movementTarget.GetTarget(), transform);
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
        World world = _packetWorld != null ? _packetWorld : ControllerPacketServices.GameWorld;
        if (world == null || DomainPacketServices.LocalPlayer == null)
            return;

        // Engine OnMoveToPawn uses the packet's pawn directly. It does not require
        // that pawn to be the UI-selected target. During replay the first attacked
        // goblin is still selected while this packet already follows the second one.
        Entity target = world.GetEntityNoLockSync(moveToPawnPacket.TarObjid);
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
        bool caveHole = WorldPlacement.IsDarkElfCave(transform.position)
            && !WorldPlacement.HasFloorWithin(transform.position, WorldPlacement.CaveDeckMeters);
        if (_controller.isGrounded)
        {
            _verticalVelocity = -2f;
        }
        else if (caveHole)
        {
            // Street mesh does not cover this spot. The next collider is the shell ~9 m down.
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
        if (!WorldPlacement.TryLiftOutOfFloor(transform.position, out float deck))
            return;
        Vector3 p = transform.position;
        p.y = deck;
        WorldPlacement.ApplyPose(gameObject, p, transform.rotation);
        _verticalVelocity = -2f;
    }

    private static float FrameIndependentLerp(float perFrameFraction, float deltaTime)
    {
        if (perFrameFraction <= 0f) return 0f;
        if (perFrameFraction >= 1f) return 1f;
        float rate = -60f * Mathf.Log(1f - perFrameFraction);
        return 1f - Mathf.Exp(-rate * deltaTime);
    }
}
