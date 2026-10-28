using System;

/// <summary>
/// Quest wire packet handler access via GameIncomingPacketServices (#142 sub-slice 62).
/// Quest list on <see cref="QuestListIncomingPacketServices"/> (sub-slice 134).
/// Ex show-quest-info on <see cref="ExShowQuestInfoIncomingPacketServices"/> (sub-slice 135).
/// </summary>
public static class QuestIncomingPacketServices
{
    public static void Queue(Action action) => GameIncomingPacketServices.Queue(action);
}
