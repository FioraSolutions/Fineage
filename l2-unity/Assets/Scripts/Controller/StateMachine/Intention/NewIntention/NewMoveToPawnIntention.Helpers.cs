using System.IO;
using UnityEngine;
using FMOD.Studio;

public partial class NewMoveToPawnIntention : IntentionBase
{
    private void StartAnimMoveTo()
    {
        if (_stateMachine.State == PlayerState.IDLE ||
            _stateMachine.State == PlayerState.ATTACKING ||
            _stateMachine.State == PlayerState.PHYSICAL_SKILLS ||
            _stateMachine.State == PlayerState.MAGIC_SKILLS ||
            _stateMachine.State == PlayerState.SITTING ||
            _stateMachine.State == PlayerState.SIT_WAIT ||
            _stateMachine.State == PlayerState.STANDING)
        {
            if (DomainPacketServices.LocalPlayer != null)
                DomainPacketServices.LocalPlayer.IsAttack = false;

            PlayerController mover = ControllerPacketServices.Player;
            bool keepRun = DomainPacketServices.LocalPlayer != null
                && DomainPacketServices.LocalPlayer.Running
                && mover != null && mover.KeepsRunGait();
            _stateMachine.ChangeState(keepRun ? PlayerState.RUNNING : PlayerState.WALKING);
            _stateMachine.NotifyEvent(Event.MOVE_TO);
        }
        else if (_stateMachine.State == PlayerState.WALKING ||
                 _stateMachine.State == PlayerState.RUNNING)
        {
            _stateMachine.NotifyEvent(Event.MOVE_TO);
        }
    }

    public override void Exit() { }

    public override void Update()
    {

    }
}
