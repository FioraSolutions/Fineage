using System;

/// <summary>
/// Ex show-quest-info wire packet handler entry (#142 sub-slice 135).
/// Splits ex quest-info apply out of <see cref="QuestIncomingPacketServices"/> (sub-slice 62 encapsulation).
/// </summary>
public static class ExShowQuestInfoIncomingPacketServices
{
    public static void Queue(Action action) => GameIncomingPacketServices.Queue(action);

    public static void ApplyExShowQuestInfo(ExShowQuestInfoDto packet)
    {
        if (IncomingPacketServices.GameWorld == null)
            return;

        Queue(() => GameIncomingPacketServices.QuestList.ShowWindow());
    }
}
