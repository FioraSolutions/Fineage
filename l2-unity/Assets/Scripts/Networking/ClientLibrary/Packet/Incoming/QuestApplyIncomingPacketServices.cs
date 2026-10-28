using System;

/// <summary>
/// Quest UI apply hub (#142 batch slice 160; locator slice 170).
/// </summary>
public static class QuestApplyIncomingPacketServices
{
    public static QuestWindow Quest => LocatorApplyIncomingPacketServices.Quest;

    public static QuestListWindow QuestList => LocatorApplyIncomingPacketServices.QuestList;

    public static void Queue(Action action) =>
        QueueApplyIncomingPacketServices.Queue(action);
}
