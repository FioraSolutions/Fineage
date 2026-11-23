public class NewDeadState : StateBase
{
    public NewDeadState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        IPlayerPawn pawn = ControllerPacketServices.Player;
        if (pawn.RunningToDestination)
            pawn.StopMove();
    }
    public override void HandleEvent(Event evt, object payload = null)
    {
        switch (evt)
        {
            case Event.DEAD:
                ControllerPacketServices.Animations.PlayOriginalAnimation(_stateMachine.GetObjectId() , AnimationNames.DEAD.ToString());
                break;
 
        }
    }
}