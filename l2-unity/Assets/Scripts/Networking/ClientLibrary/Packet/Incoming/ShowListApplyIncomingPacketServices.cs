using System;

/// <summary>
/// Show-list UI apply hub (#142 batch slice 162; locator slice 170).
/// </summary>
public static class ShowListApplyIncomingPacketServices
{
    public static ShowListWindow ShowList => IncomingPacketLocatorApplyIncomingPacketServices.ShowList;

    public static void Queue(Action action) =>
        QueueApplyIncomingPacketServices.Queue(action);
}
