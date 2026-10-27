using UnityEngine;

[IncomingGamePacket(GameServerPacketType.MultiSellList)]
public sealed class MultiSellListIncoming : IncomingWirePacket<MultiSellListDto>
{
    public override void Apply(MultiSellListDto packet)
    {
        IncomingPacketServices.Queue(() =>
        {
            IncomingPacketServices.MultiSell.AddData(packet.GetOnlyItems(), packet);
            IncomingPacketServices.MultiSell.ShowWindow();
        });
    }
}
