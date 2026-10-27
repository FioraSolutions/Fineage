using UnityEngine;

[IncomingGamePacket(GameServerPacketType.SendTradeRequest)]
public sealed class SendTradeRequestIncoming : IncomingWirePacket<SendTradeRequestDto>
{
    public override void Apply(SendTradeRequestDto packet)
    {
        IncomingPacketServices.Queue(() =>
        {
            IncomingPacketServices.TradeRequest.AddData(packet);
            IncomingPacketServices.TradeRequest.ShowWindow();
        });
    }
}
