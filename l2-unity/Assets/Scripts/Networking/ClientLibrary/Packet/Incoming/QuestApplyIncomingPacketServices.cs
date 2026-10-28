using System;

/// <summary>
/// Quest UI apply hub (#142 batch slice 160; game hub slice 176).
/// </summary>
public static class QuestApplyIncomingPacketServices
{
    public static QuestWindow Quest => LocatorApplyIncomingPacketServices.Quest;

    public static QuestListWindow QuestList => LocatorApplyIncomingPacketServices.QuestList;

    public static void Queue(Action action) =>
        GameApplyIncomingPacketServices.Queue(action);
}
