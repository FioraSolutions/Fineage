using UnityEngine;

[IncomingGamePacket(GameServerPacketType.TradeStart)]
public sealed class TradeStartIncoming : IncomingWirePacket<TradeStartDto>
{
    public override void Apply(TradeStartDto packet)
    {
        GameIncomingPacketServices.Queue(() =>
        {
            GameIncomingPacketServices.Trade.AddData(packet);
            GameIncomingPacketServices.Trade.ShowWindow();
        });
    }
}
