using System;

/// <summary>
/// Apply facade (#142 batch slice 151; locator queue slice 260). Re-homes from <see cref="ExShowQuestInfoIncomingPacketServices"/>.
/// 
/// Ex show-quest-info wire packet handler entry (#142 sub-slice 135).
/// Splits ex quest-info apply out of <see cref="QuestIncomingPacketServices"/> (sub-slice 62 encapsulation).
/// </summary>
public static class ExShowQuestInfoApplyIncomingPacketServices
{
    public static QuestListWindow QuestList => LocatorApplyIncomingPacketServices.QuestList;

    public static void Queue(Action action) => QueueApplyIncomingPacketServices.Queue(action);

    public static void ApplyExShowQuestInfo(ExShowQuestInfoDto packet)
    {
        if (LocatorApplyIncomingPacketServices.GameWorld == null)
            return;

        Queue(() => QuestList.ShowWindow());
    }
}
