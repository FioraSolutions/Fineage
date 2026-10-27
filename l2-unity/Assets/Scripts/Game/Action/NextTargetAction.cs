public class NextTargetAction : L2Action
{
    private TargetManager _targets;

    public void Bind(TargetManager targets)
    {
        _targets = targets;
    }

    private TargetManager Targets
    {
        get { return _targets != null ? _targets : IncomingPacketActions.Targets; }
    }

    public override void UseAction()
    {
        TargetManager targets = Targets;
        if (targets != null)
            targets.NextTarget();
    }
}
