using System;

/// <summary>
/// Apply facade (#142 batch slice 151; locator queue slice 255). Re-homes from <see cref="ExShowQuestInfoIncomingPacketServices"/>.
/// 
/// Ex show-quest-info wire packet handler entry (#142 sub-slice 135).
/// Splits ex quest-info apply out of <see cref="QuestIncomingPacketServices"/> (sub-slice 62 encapsulation).
/// </summary>
public static class ExShowQuestInfoApplyIncomingPacketServices
{
    public static void Queue(Action action) => QueueApplyIncomingPacketServices.Queue(action);

    public static void ApplyExShowQuestInfo(ExShowQuestInfoDto packet)
    {
        if (WorldApplyIncomingPacketServices.GameWorld == null)
            return;

        Queue(() => LocatorApplyIncomingPacketServices.QuestList.ShowWindow());
    }
}
