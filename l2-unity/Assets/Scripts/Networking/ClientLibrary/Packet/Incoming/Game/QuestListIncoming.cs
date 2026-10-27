using UnityEngine;

[IncomingGamePacket(GameServerPacketType.QuestList)]
public sealed class QuestListIncoming : IncomingWirePacket<QuestListDto>
{
    public override void Apply(QuestListDto packet)
    {
        GameIncomingPacketServices.Queue(() => GameIncomingPacketServices.Quest.AddData(packet.Quest));
    }
}
