using System;

/// <summary>
/// Quest wire packet handler access via GameIncomingPacketServices (#142 sub-slice 62).
/// </summary>
public static class QuestIncomingPacketServices
{
    public static void Queue(Action action) => GameIncomingPacketServices.Queue(action);

    public static void ApplyQuestList(QuestListDto packet) =>
        Queue(() => GameIncomingPacketServices.Quest.AddData(packet.Quest));

    public static void ApplyExShowQuestInfo(ExShowQuestInfoDto packet)
    {
        if (IncomingPacketServices.GameWorld == null)
            return;

        Queue(() => GameIncomingPacketServices.QuestList.ShowWindow());
    }
}
