

using UnityEngine;

public class StandIntention : IntentionBase
{
    public StandIntention(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter(object arg0)
    {
        // Mobius RequestActionUse 0 again: the zone toggles and answers ChangeWaitType(WT_STANDING).
        NetSession.Ensure().SendActionUse(0);
    }

    public override void Exit() { }
    public override void Update()
    {
        //if (_stateMachine.WaitingForServerReply)
        //{
        //    PlayerController.Instance.StopMoving();
        //}
    }
}