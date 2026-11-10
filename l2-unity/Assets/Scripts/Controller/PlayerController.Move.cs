using System;

using UnityEngine;

public partial class PlayerController
{
    public void RestartMoveElseMoveToPawnTrue(ref bool _isMove)
    {
        PlayerStateMachine stateMachine = PlayerStatePacketServices.PlayerState;
        if (stateMachine == null || _movementTarget == null)
            return;

        if (stateMachine.IsMoveToPawn & _isMove == false)
        {
            Vector3 targetPosition = VectorUtils.To2D(_movementTarget.GetTarget());
            float distance = VectorUtils.Distance2D(transform.position, targetPosition);
            if (distance > _movementTarget.GetDistance())
            {
                // Debug.Log("IsMoveToPawn запуск повтора т.е IsMoveToPawn " + PlayerStatePacketServices.PlayerState.IsMoveToPawn + " state " + PlayerStatePacketServices.PlayerState.State + " Distance " + distance);
                _isMove = true;
            }
        }
    }

   


    private void SetPositionServer(MovementTarget _movementTarget , Vector3 targetPosition)
    {
        if (_movementTarget.GetDistance() == 0.1f)
        {
            var _targetPosition = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
            transform.position = _targetPosition;
        }
    }
    private void SwitchWalkToRun()
    {
        if (_switchWalkToRun)
            return;
        if (L2PlayerMoveGait.IsWalkStart(_moveTimer, RunMode(), false))
            return;

        _switchWalkToRun = true;
        PlayerStateMachine stateMachine = PlayerStatePacketServices.PlayerState;
        if (stateMachine == null)
            return;
        stateMachine.ChangeState(PlayerState.RUNNING);
        stateMachine.NotifyEvent(Event.MOVE_TO);
    }

    private bool RunMode()
    {
        if (DomainPacketServices.LocalPlayer != null)
            return DomainPacketServices.LocalPlayer.Running;
        return _movementTarget != null && _movementTarget.IsRunningServer();
    }

    private float GetMovementSpeed()
    {
        bool walking = L2PlayerMoveGait.IsWalkStart(_moveTimer, RunMode(), false);
        return _calcSpeed.GetSpeed(!walking);
    }



  

 
    private Vector3 GetNewPosition(Vector3 targetPosition, Vector3 characterPosition , float currentSpeed)
    {

        Vector3 relativeDirection = targetPosition - characterPosition;
        return relativeDirection.normalized * currentSpeed;
    }



 

    public void MoveToPoint(MovementTarget movementTarget)
    {
        bool wasMoving = _isMove;
        ActivateIfNotMoving(_isMove);
        if (!wasMoving)
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
        _moveTimer = 0f;
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

    
    /// <summary>
    /// Vertical velocity integrator with Interlude physWalking / physFalling semantics.
    /// Grounded: planar velocity — Physics isGrounded holds the pawn on the floor, a
    /// small downward bias keeps the CharacterController touching it over slopes.
    /// Airborne: accumulate gravity, capped at terminal velocity.
    /// Returns the Y velocity (m/s); the caller integrates it with dt exactly once.
    /// </summary>
    private float ApplyGravity(float deltaTime)
    {
        if (_controller.isGrounded)
        {
            _verticalVelocity = -2f;
        }
        else
        {
            _verticalVelocity -= _gravity * deltaTime;
            if (_verticalVelocity < -TerminalVelocity)
                _verticalVelocity = -TerminalVelocity;
        }
        return _verticalVelocity;
    }

    /// <summary>
    /// Frame-rate independent lerp factor preserving the legacy per-frame feel
    /// (0.06 / 0.2 fractions) at the game's 60 fps target. Without this,
    /// Quaternion.Lerp(t) sped up or slowed down with the frame rate.
    /// </summary>
    private static float FrameIndependentLerp(float perFrameFraction, float deltaTime)
    {
        if (perFrameFraction <= 0f) return 0f;
        if (perFrameFraction >= 1f) return 1f;
        float rate = -60f * Mathf.Log(1f - perFrameFraction);
        return 1f - Mathf.Exp(-rate * deltaTime);
    }

   





