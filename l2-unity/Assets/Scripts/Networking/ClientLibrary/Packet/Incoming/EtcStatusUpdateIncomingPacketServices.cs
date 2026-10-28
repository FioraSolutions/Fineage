using System;

/// <summary>
/// Etc status wire packet handler entry (#142 batch slice 140).
/// </summary>
public static class EtcStatusUpdateIncomingPacketServices
{
    public static void Queue(Action action) => UiIncomingPacketServices.Queue(action);

    public static void ApplyEtcStatusUpdate(EtcStatusUpdateDto packet) =>
        Queue(() =>
        {
            if (UiIncomingPacketServices.Buffer != null)
                UiIncomingPacketServices.Buffer.RefreshPenalty(packet);
        });
}
