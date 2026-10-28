using System;

/// <summary>
/// Show-list window apply hub (#142 batch slice 162).
/// </summary>
public static class ShowListApplyIncomingPacketServices
{
    public static ShowListWindow ShowList => UiApplyIncomingPacketServices.ShowList;

    public static void Queue(Action action) =>
        IncomingPacketQueueApplyIncomingPacketServices.Queue(action);
}
