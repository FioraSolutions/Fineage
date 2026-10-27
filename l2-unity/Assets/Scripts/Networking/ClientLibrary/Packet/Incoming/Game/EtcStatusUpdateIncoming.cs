using UnityEngine;

[IncomingGamePacket(GameServerPacketType.EtcStatusUpdate)]
public sealed class EtcStatusUpdateIncoming : IncomingWirePacket<EtcStatusUpdateDto>
{
    public override void Apply(EtcStatusUpdateDto packet)
    {
        IncomingPacketServices.Queue(() =>
        {
            if (IncomingPacketServices.Buffer != null)
                IncomingPacketServices.Buffer.RefreshPenalty(packet);
        });
    }
}
