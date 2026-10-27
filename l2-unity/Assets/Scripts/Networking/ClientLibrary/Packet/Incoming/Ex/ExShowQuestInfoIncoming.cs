using UnityEngine;

[IncomingExPacket(GameExServerPacketType.ExShowQuestInfo)]
public sealed class ExShowQuestInfoIncoming : IncomingWirePacket<ExShowQuestInfoDto>
{
    public override void Apply(ExShowQuestInfoDto packet)
    {
        if (IncomingPacketServices.GameWorld != null)
            IncomingPacketServices.Queue(() => IncomingPacketServices.QuestList.ShowWindow());

        // Debug.Log("Event Open ExShowQuestInfo Info");
    }
}
