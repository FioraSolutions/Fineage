using FMOD.Studio;
using System.IO;
using UnityEngine;

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

            _stateMachine.ChangeState(PlayerState.WALKING);
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
