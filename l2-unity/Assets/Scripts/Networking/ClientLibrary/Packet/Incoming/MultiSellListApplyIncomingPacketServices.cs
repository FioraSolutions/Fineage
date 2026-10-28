using System;

/// <summary>
/// Apply facade (#142 batch slice 151). Re-homes from <see cref="MultiSellListIncomingPacketServices"/>.
/// 
/// Multi-sell list wire packet handler entry (#142 batch slice 145).
/// Re-homes apply from <see cref="MultiSellIncomingPacketServices"/> (sub-slice 74).
/// </summary>
public static class MultiSellListApplyIncomingPacketServices
{
    public static void Queue(Action action) => UiIncomingPacketServices.Queue(action);

    public static void ApplyMultiSellList(MultiSellListDto packet) =>
        Queue(() =>
        {
            UiIncomingPacketServices.MultiSell.AddData(packet.GetOnlyItems(), packet);
            UiIncomingPacketServices.MultiSell.ShowWindow();
        });
}
