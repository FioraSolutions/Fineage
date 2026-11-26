
public class SitStandAction : L2Action
{
    public SitStandAction() : base() { }

    // Local action
    public override void UseAction()
    {
        // Mobius action 0 → zone toggles sit/stand → ChangeWaitType (Sit action, Relax, chat /sit).
        InterludeRequestActionUse.TrySend((int)ActionType.Sit);
    }
}