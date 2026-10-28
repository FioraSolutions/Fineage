using System;

/// <summary>
/// Quest UI apply hub (#142 batch slice 160; game hub slice 167).
/// </summary>
public static class QuestApplyIncomingPacketServices
{
    public static QuestWindow Quest => GameApplyIncomingPacketServices.Quest;

    public static QuestListWindow QuestList => GameApplyIncomingPacketServices.QuestList;

    public static void Queue(Action action) => GameApplyIncomingPacketServices.Queue(action);
}
