using UnityEngine;

public class RideStriderAction : L2Action
{
    public override void UseAction()
    {
        if (PlayerStatePacketServices.PlayerState == null)
            return;
        if (PlayerStatePacketServices.PlayerState.State == PlayerState.DEAD)
            return;
        // The action button is Interlude Mount/Dismount. It must not summon a strider
        // when the player has none — that read as a "sit on a strider" shortcut.
        StriderRideService ride = AnimationPacketServices.StriderRide;
        PlayerEntity player = DomainPacketServices.LocalPlayer;
        if (ride == null || player == null || !ride.IsRiding(player))
            return;
        ride.Dismount(player);
        PlayerStatePacketServices.PlayerState.NotifyEvent(Event.RIDE);
    }
}
