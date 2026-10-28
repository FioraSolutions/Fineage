using System;

/// <summary>
/// Quest UI apply hub (#142 batch slice 160).
/// </summary>
public static class QuestApplyIncomingPacketServices
{
    public static QuestWindow Quest => IncomingPacketLocatorApplyIncomingPacketServices.Quest;

    public static QuestListWindow QuestList => IncomingPacketLocatorApplyIncomingPacketServices.QuestList;

    public static void Queue(Action action) =>
        IncomingPacketQueueApplyIncomingPacketServices.Queue(action);
}
