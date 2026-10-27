using System;

/// <summary>
/// Multi-sell list wire packet handler access via UiIncomingPacketServices (#142 sub-slice 74).
/// </summary>
public static class MultiSellIncomingPacketServices
{
    public static void Queue(Action action) => UiIncomingPacketServices.Queue(action);

    public static void ApplyMultiSellList(MultiSellListDto packet) =>
        Queue(() =>
        {
            UiIncomingPacketServices.MultiSell.AddData(packet.GetOnlyItems(), packet);
            UiIncomingPacketServices.MultiSell.ShowWindow();
        });
}
