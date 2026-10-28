public class DeadState : StateBase
{
    public DeadState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        ControllerPacketServices.Player.StopMove();
    }
    public override void HandleEvent(Event evt, object payload = null)
    {
        switch (evt)
        {
            case Event.DEAD:
                ControllerPacketServices.Animations.PlayAnimation(_stateMachine.Player.Identity.Id , AnimationNames.RUN.ToString(), true);
                break;
        }
    }
}