using System;

/// <summary>
/// Quest wire packet handler access via GameIncomingPacketServices (#142 sub-slice 62).
/// Quest list on <see cref="QuestListIncomingPacketServices"/> (sub-slice 134).
/// </summary>
public static class QuestIncomingPacketServices
{
    public static void Queue(Action action) => GameIncomingPacketServices.Queue(action);

    public static void ApplyExShowQuestInfo(ExShowQuestInfoDto packet)
    {
        if (IncomingPacketServices.GameWorld == null)
            return;

        Queue(() => GameIncomingPacketServices.QuestList.ShowWindow());
    }
}
