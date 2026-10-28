using System;

/// <summary>
/// Quest list wire packet handler entry (#142 sub-slice 134).
/// Splits quest-list apply out of <see cref="QuestIncomingPacketServices"/> (sub-slice 62 encapsulation).
/// </summary>
public static class QuestListIncomingPacketServices
{
    public static void Queue(Action action) => GameIncomingPacketServices.Queue(action);

    public static void ApplyQuestList(QuestListDto packet) =>
        Queue(() => GameIncomingPacketServices.Quest.AddData(packet.Quest));
}
