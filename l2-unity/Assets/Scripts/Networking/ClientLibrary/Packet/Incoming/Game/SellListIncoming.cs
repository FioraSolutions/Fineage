using UnityEngine;

[IncomingGamePacket(GameServerPacketType.SellList)]
public sealed class SellListIncoming : IncomingWirePacket<SellListDto>
{
    public override void Apply(SellListDto packet)
    {
        MerchantIncomingPacketServices.ApplySellList(packet);
    }
}
