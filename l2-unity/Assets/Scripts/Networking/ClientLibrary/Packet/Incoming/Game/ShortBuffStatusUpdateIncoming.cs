using UnityEngine;

[IncomingGamePacket(GameServerPacketType.ShortBuffStatusUpdate)]
public sealed class ShortBuffStatusUpdateIncoming : IncomingWirePacket<ShortBuffStatusUpdateDto>
{
    public override void Apply(ShortBuffStatusUpdateDto packet)
    {
        UiIncomingPacketServices.Queue(() =>
        {
            if (UiIncomingPacketServices.Buffer == null || packet.Effect == null)
                return;
            var item = packet.Effect;
            UiIncomingPacketServices.Buffer.AddDataCellToTime(item.Id, item.Value, item.Duration);
        });
    }
}
