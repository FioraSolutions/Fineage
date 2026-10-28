[IncomingExPacket(GameExServerPacketType.ExShowQuestInfo)]
public sealed class ExShowQuestInfoIncoming : IncomingWirePacket<ExShowQuestInfoDto>
{
    public override void Apply(ExShowQuestInfoDto packet) =>
        ExShowQuestInfoIncomingPacketServices.ApplyExShowQuestInfo(packet);
}
