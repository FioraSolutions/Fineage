using UnityEngine;

[IncomingGamePacket(GameServerPacketType.ShortBuffStatusUpdate)]
public sealed class ShortBuffStatusUpdateIncoming : IncomingWirePacket<ShortBuffStatusUpdateDto>
{
    public override void Apply(ShortBuffStatusUpdateDto packet)
    {
        IncomingPacketServices.Queue(() =>
        {
            if (IncomingPacketServices.Buffer == null || packet.Effect == null)
                return;
            var item = packet.Effect;
            IncomingPacketServices.Buffer.AddDataCellToTime(item.Id, item.Value, item.Duration);
        });
    }
}
