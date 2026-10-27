using UnityEngine;

[IncomingGamePacket(GameServerPacketType.AbnormalStatusUpdate)]
public sealed class AbnormalStatusUpdateIncoming : IncomingWirePacket<AbnormalStatusUpdateDto>
{
    public override void Apply(AbnormalStatusUpdateDto packet)
    {
        IncomingPacketServices.Queue(() =>
        {
            if (IncomingPacketServices.Buffer == null)
                return;
            foreach (var item in packet.ListEffect)
                IncomingPacketServices.Buffer.AddDataCellToTime(item.Id, item.Value, item.Duration);
        });
    }
}
