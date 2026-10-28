using System;

/// <summary>
/// Quest UI apply hub (#142 batch slice 160; locator slice 170).
/// </summary>
public static class QuestApplyIncomingPacketServices
{
    public static QuestWindow Quest => IncomingPacketLocatorApplyIncomingPacketServices.Quest;

    public static QuestListWindow QuestList => IncomingPacketLocatorApplyIncomingPacketServices.QuestList;

    public static void Queue(Action action) =>
        QueueApplyIncomingPacketServices.Queue(action);
}
