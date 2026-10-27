using UnityEngine;

[IncomingGamePacket(GameServerPacketType.TradeStart)]
public sealed class TradeStartIncoming : IncomingWirePacket<TradeStartDto>
{
    public override void Apply(TradeStartDto packet)
    {
        IncomingPacketServices.Queue(() =>
        {
            IncomingPacketServices.Trade.AddData(packet);
            IncomingPacketServices.Trade.ShowWindow();
        });
    }
}
