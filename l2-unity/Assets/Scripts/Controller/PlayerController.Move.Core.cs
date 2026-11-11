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
}
