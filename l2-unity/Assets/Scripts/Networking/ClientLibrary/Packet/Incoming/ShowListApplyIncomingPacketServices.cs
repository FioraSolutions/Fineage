using System;

/// <summary>
/// Show-list UI apply hub (#142 batch slice 162; UI hub slice 176).
/// </summary>
public static class ShowListApplyIncomingPacketServices
{
    public static ShowListWindow ShowList => LocatorApplyIncomingPacketServices.ShowList;

    public static void Queue(Action action) =>
        QueueApplyIncomingPacketServices.Queue(action);
}
