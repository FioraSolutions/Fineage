using UnityEngine;

[IncomingGamePacket(GameServerPacketType.SendTradeRequest)]
public sealed class SendTradeRequestIncoming : IncomingWirePacket<SendTradeRequestDto>
{
    public override void Apply(SendTradeRequestDto packet)
    {
        GameIncomingPacketServices.Queue(() =>
        {
            GameIncomingPacketServices.TradeRequest.AddData(packet);
            GameIncomingPacketServices.TradeRequest.ShowWindow();
        });
    }
}
