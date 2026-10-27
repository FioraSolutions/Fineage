using UnityEngine;

[IncomingExPacket(GameExServerPacketType.ExShowQuestInfo)]
public sealed class ExShowQuestInfoIncoming : IncomingWirePacket<ExShowQuestInfoDto>
{
    public override void Apply(ExShowQuestInfoDto packet)
    {
        QuestIncomingPacketServices.ApplyExShowQuestInfo(packet);

        // Debug.Log("Event Open ExShowQuestInfo Info");
    }
}
