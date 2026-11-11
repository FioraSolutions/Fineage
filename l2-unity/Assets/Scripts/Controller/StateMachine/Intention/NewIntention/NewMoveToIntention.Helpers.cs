using System.Security.Cryptography;
using UnityEngine;
using System.IO;

public partial class NewMoveToIntention : IntentionBase
{
    void StandIfSeated()
    {
        if (_stateMachine.State != PlayerState.SITTING &&
            _stateMachine.State != PlayerState.SIT_WAIT &&
            _stateMachine.State != PlayerState.STANDING)
            return;

        _stateMachine.ChangeState(PlayerState.IDLE);
        _stateMachine.NotifyEvent(Event.ARRIVED);
    }

    public override void Exit() { }

    public override void Update()
    {

    }
}
