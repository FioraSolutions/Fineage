using Fineage.Net.Contract;

public sealed partial class NetSession
{
    const int RideMountAction = 1;
    const int RideDismountAction = 2;

    internal void ApplyRide(RideNetMsg msg)
    {
        if (msg == null)
            return;

        QueueApplyIncomingPacketServices.Queue(() =>
        {
            World world = WorldApplyPacketServices.GameWorld;
            if (world == null)
            {
                FineageLog.Error(FineageLogCategory.Net, "ApplyRide: GameWorld is null.");
                return;
            }

            Entity entity = world.GetEntityNoLockSync(msg.ObjectId);
            if (entity == null)
                return;

            StriderRideService ride = AnimationPacketServices.StriderRide;
            if (ride == null)
            {
                FineageLog.Error(FineageLogCategory.Net, "ApplyRide: StriderRideService is not bound.");
                return;
            }

            if (msg.Action == RideMountAction)
                ride.TryMount(entity);
            else if (msg.Action == RideDismountAction)
                ride.Dismount(entity);
        });
    }

    internal void ApplyPlayerActionState(PlayerActionStateNetMsg msg)
    {
        if (msg == null)
            return;

        QueueApplyIncomingPacketServices.Queue(() =>
            ActionAvailabilityPacketServices.Availability.ApplyPlayerActionState(
                msg.PetOut,
                msg.Mounted,
                msg.MountKind,
                msg.PetControlObjectId));
    }
}