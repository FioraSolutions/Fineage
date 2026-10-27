using UnityEngine;

[IncomingGamePacket(GameServerPacketType.QuestList)]
public sealed class QuestListIncoming : IncomingWirePacket<QuestListDto>
{
    public override void Apply(QuestListDto packet)
    {
        IncomingPacketServices.Queue(() => IncomingPacketServices.Quest.AddData(packet.Quest));
    }
}
