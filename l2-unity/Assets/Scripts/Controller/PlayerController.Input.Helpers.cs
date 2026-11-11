using System;

using UnityEngine;

public partial class PlayerController : MonoBehaviour
{
    public void Update()
    {
        if (_isRotateAttacker & !_isMove)
        {
            if (!VectorUtils.IsFacingAttacker(transform , _attackerPosition , angleThreshold))
            {
               // Debug.Log("Rotate to object Attacker running : ");
                TurnTowardsAttacker(_attackerPosition);
            }
            else
            {
               //Debug.Log("Rotate to object Attacker stop: ");
                _isRotateAttacker = false;
            }

        }

        StepLocalMove();
    }

    void StepLocalMove()
    {
        if (_controller == null || !_controller.enabled)
            return;

        //Если мы не двигаемся но мы в состоянии MoveToPawnDto == true тогда мы следим за обьектом двинется он или нет если двинется мы подбегаем к нему
        //Если мы получим MoveToPawnDto == false мы останавливаем слежение
        RestartMoveElseMoveToPawnTrue(ref _isMove);

        if (!_isMove)
            return;

        Vector3 characterPosition = VectorUtils.To2D(transform.position);
        Vector3 targetPosition = VectorUtils.To2D(_movementTarget.GetTarget());

        if(VectorUtils.Distance2D(characterPosition, targetPosition) > _movementTarget.GetDistance())
        {

            _moveTimer += Time.deltaTime;
            SwitchWalkToRun();

            float finalAngle = GetFinalAngle(targetPosition, characterPosition);

            float speed = GetMovementSpeed();
            Vector3 velocity = GetNewPosition(targetPosition, characterPosition, speed);

            // APawn::calcVelocity arrival clamp (Pawn.h): when the remaining
            // distance is below this frame's step, Velocity = Dest / DeltaTime so
            // the pawn lands on the destination instead of overshooting and
            // snapping back on the next frame.
            Vector3 toTarget = targetPosition - characterPosition;
            float toTargetDist = toTarget.magnitude;
            float stepLength = speed * Time.deltaTime;
            if (toTargetDist > 0.0001f && toTargetDist < stepLength)
                velocity = toTarget / Time.deltaTime;

            velocity.y = ApplyGravity(Time.deltaTime);

            Quaternion targetRotation = GetNewRotate(finalAngle);
            float r_speed = _calcSpeed.GetSpeedRotate(_behindPlayer);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, FrameIndependentLerp(r_speed, Time.deltaTime));

            _controller.Move(velocity * Time.deltaTime);


            _isFirst = false;
            _pp_sender.TickWhileMoving(transform.position, PlayerPositionSender.ToL2Heading(transform.eulerAngles.y), Time.deltaTime);
        }
        else
        {
             //Debug.Log("IsMoveToPawn мы на месте " + VectorUtils.Distance2D(characterPosition, targetPosition));
             SetPositionServer(_movementTarget, targetPosition);
             NotifyEvent(PlayerStatePacketServices.PlayerState.State);
             StopMove();

           // Debug.Log("IsMoveToPawn запуск повтора т.е StopMove " + PlayerStatePacketServices.PlayerState.IsMoveToPawn + " state " + PlayerStatePacketServices.PlayerState.State);
        }
    }

    private void NotifyEvent(PlayerState state)
    {
        PlayerStateMachine stateMachine = PlayerStatePacketServices.PlayerState;
        if (stateMachine == null)
            return;

        if (state == PlayerState.RUNNING | state == PlayerState.WALKING)
        {
            stateMachine.NotifyEvent(Event.ARRIVED);
        }
    }
}
