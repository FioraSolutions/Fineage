using UnityEngine;

public class SitStandAction : L2Action
{
    public SitStandAction() : base() { }

    // Local action
    public override void UseAction()
    {
        if (PlayerStatePacketServices.PlayerState.State == PlayerState.DEAD)
        {
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