using UnityEngine;

public class RideStriderAction : L2Action
{
    public override void UseAction()
    {
        if (PlayerStatePacketServices.PlayerState == null)
            return;
        if (PlayerStatePacketServices.PlayerState.State == PlayerState.DEAD)
            return;
        PlayerStatePacketServices.PlayerState.ToggleRide();
    }
}
