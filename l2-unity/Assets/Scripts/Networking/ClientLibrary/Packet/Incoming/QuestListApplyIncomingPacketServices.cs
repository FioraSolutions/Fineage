using System;

/// <summary>
/// Apply facade (#142 batch slice 151). Re-homes from <see cref="QuestListIncomingPacketServices"/>.
/// 
/// Quest list wire packet handler entry (#142 sub-slice 134).
/// Splits quest-list apply out of <see cref="QuestIncomingPacketServices"/> (sub-slice 62 encapsulation).
/// </summary>
public static class QuestListApplyIncomingPacketServices
{
    public static void Queue(Action action) => QuestApplyIncomingPacketServices.Queue(action);

    public static void ApplyQuestList(QuestListDto packet) =>
        Queue(() => QuestApplyIncomingPacketServices.Quest.AddData(packet.Quest));
}
