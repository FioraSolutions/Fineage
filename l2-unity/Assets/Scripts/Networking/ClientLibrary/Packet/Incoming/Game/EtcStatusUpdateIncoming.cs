using UnityEngine;

[IncomingGamePacket(GameServerPacketType.EtcStatusUpdate)]
public sealed class EtcStatusUpdateIncoming : IncomingWirePacket<EtcStatusUpdateDto>
{
    public override void Apply(EtcStatusUpdateDto packet)
    {
        UiIncomingPacketServices.Queue(() =>
        {
            if (UiIncomingPacketServices.Buffer != null)
                UiIncomingPacketServices.Buffer.RefreshPenalty(packet);
        });
    }
}
