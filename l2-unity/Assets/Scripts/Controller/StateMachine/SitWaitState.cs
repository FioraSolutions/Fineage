public class SitWaitState : StateBase
{
    public SitWaitState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void HandleEvent(Event evt, object payload = null)
    {
        // ACTION_ALLOWED / stand transition previously lived here; kept empty until rewired.
    }
}
