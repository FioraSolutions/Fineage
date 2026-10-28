
[IncomingGamePacket(GameServerPacketType.QuestList)]
public sealed class QuestListIncoming : IncomingWirePacket<QuestListDto>
{
    public override void Apply(QuestListDto packet) =>
        QuestListIncomingPacketServices.ApplyQuestList(packet);
}
