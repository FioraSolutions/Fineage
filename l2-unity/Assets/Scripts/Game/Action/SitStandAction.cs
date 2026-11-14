
public class SitStandAction : L2Action
{
    public SitStandAction() : base() { }

    // Local action
    public override void UseAction()
    {
        if (PlayerStatePacketServices.PlayerState == null ||
            PlayerStatePacketServices.PlayerState.State == PlayerState.DEAD)
        {
            return;
        }

        NetSession session = NetSession.TryGetBound();
        if (session != null)
        {
            // Zone toggles sit/stand and fans ChangeWaitType, which plays the sit clip.
            _ = session.SendActionUse((int)ActionType.Sit);
            return;
        }

        if (PlayerStatePacketServices.PlayerState.State == PlayerState.SITTING || PlayerStatePacketServices.PlayerState.State == PlayerState.SIT_WAIT)
        {
            PlayerStatePacketServices.PlayerState.ChangeIntention(Intention.INTENTION_STAND);
        }
        else
        {
            PlayerStatePacketServices.PlayerState.ChangeIntention(Intention.INTENTION_SIT);
        }
    }
}