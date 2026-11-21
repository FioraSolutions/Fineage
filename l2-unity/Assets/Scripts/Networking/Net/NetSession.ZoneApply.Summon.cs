using Fineage.Net.Contract;

public sealed partial class NetSession
{
    internal void ApplySummonInfo(SummonInfoNetMsg msg)
    {
        if (msg == null)
            return;
        QueueApplyIncomingPacketServices.Queue(() =>
            ActionAvailabilityPacketServices.Availability.NoteSummon(
                (byte)msg.Kind,
                msg.NpcId,
                msg.Dead,
                msg.Mountable));
    }

    internal void ApplySummonGone(SummonGoneNetMsg msg)
    {
        if (msg == null)
            return;
        QueueApplyIncomingPacketServices.Queue(() =>
            ActionAvailabilityPacketServices.Availability.NoteSummonDismissed((byte)msg.Kind));
    }
}