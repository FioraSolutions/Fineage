/// <summary>Placeholder until the action's Net packet / animation is wired (icons show in ActionWnd now).</summary>
public sealed class StubAction : L2Action
{
    readonly int _actionId;

    public StubAction(int actionId) => _actionId = actionId;

    public override void UseAction()
    {
        // Icons are live; server/animation follow-up. Still notify Net when connected so GM can see the id.
        if (ZoneCommands.IsConnected)
            _ = NetSession.Ensure().SendActionUse(_actionId);
    }
}
