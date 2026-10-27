using UnityEngine;

[IncomingGamePacket(GameServerPacketType.MultiSellList)]
public sealed class MultiSellListIncoming : IncomingWirePacket<MultiSellListDto>
{
    public override void Apply(MultiSellListDto packet)
    {
        UiIncomingPacketServices.Queue(() =>
        {
            UiIncomingPacketServices.MultiSell.AddData(packet.GetOnlyItems(), packet);
            UiIncomingPacketServices.MultiSell.ShowWindow();
        });
    }
}
