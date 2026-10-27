using static AttackingState;

public class WalkingState : StateBase
{
    public WalkingState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void HandleEvent(Event evt, object payload = null)
    {
        switch (evt)
        {
            case Event.ARRIVED:
                if (ControllerPacketServices.Targets.HasAttackTarget())
                {
                 //   _stateMachine.ChangeIntention(Intention.INTENTION_ATTACK, AttackIntentionType.TargetReached);
                }
                else
                {
                    _stateMachine.ChangeIntention(Intention.INTENTION_IDLE);
                }
                break;

        }
    }

    public override void Update()
    {
        //Arrived to destination
        if (!ControllerPacketServices.Input.Move && !ControllerPacketServices.Player.RunningToDestination)
        {
            _stateMachine.NotifyEvent(Event.ARRIVED);
        }

        // If move input is pressed while running to target
        if (ControllerPacketServices.Targets.HasAttackTarget() && ControllerPacketServices.Input.Move)
        {
            // Cancel follow target
            ControllerPacketServices.Targets.ClearAttackTarget();
        }
    }
}