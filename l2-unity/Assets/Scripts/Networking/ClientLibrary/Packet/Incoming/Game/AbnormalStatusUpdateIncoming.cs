using UnityEngine;

[IncomingGamePacket(GameServerPacketType.AbnormalStatusUpdate)]
public sealed class AbnormalStatusUpdateIncoming : IncomingWirePacket<AbnormalStatusUpdateDto>
{
    public override void Apply(AbnormalStatusUpdateDto packet)
    {
        WorldIncomingPacketServices.Queue(() =>
        {
            if (WorldIncomingPacketServices.Buffer == null)
                return;
            foreach (var item in packet.ListEffect)
                WorldIncomingPacketServices.Buffer.AddDataCellToTime(item.Id, item.Value, item.Duration);
        });
    }
}
