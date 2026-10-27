using UnityEngine;

[IncomingExPacket(GameExServerPacketType.ExShowQuestInfo)]
public sealed class ExShowQuestInfoIncoming : IncomingWirePacket<ExShowQuestInfoDto>
{
    public override void Apply(ExShowQuestInfoDto packet)
    {
        if (IncomingPacketServices.GameWorld != null)
            GameIncomingPacketServices.Queue(() => GameIncomingPacketServices.QuestList.ShowWindow());

        // Debug.Log("Event Open ExShowQuestInfo Info");
    }
}
