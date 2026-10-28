using System;

/// <summary>
/// Etc status wire packet handler entry (#142 batch slice 140).
/// Apply facade (#142 batch slice 153; locator queue slice 258). Re-homes from <see cref="EtcStatusUpdateIncomingPacketServices"/>.
/// </summary>
public static class EtcStatusUpdateApplyIncomingPacketServices
{
    public static void Queue(Action action) => QueueApplyIncomingPacketServices.Queue(action);

    public static void ApplyEtcStatusUpdate(EtcStatusUpdateDto packet) =>
        Queue(() =>
        {
            if (LocatorApplyIncomingPacketServices.Buffer != null)
                LocatorApplyIncomingPacketServices.Buffer.RefreshPenalty(packet);
        });
}
