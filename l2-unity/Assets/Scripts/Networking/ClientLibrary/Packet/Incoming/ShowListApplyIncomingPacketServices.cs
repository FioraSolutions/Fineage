using System;

/// <summary>
/// Show-list UI apply hub (#142 batch slice 162; UI hub slice 167).
/// </summary>
public static class ShowListApplyIncomingPacketServices
{
    public static ShowListWindow ShowList => UiApplyIncomingPacketServices.ShowList;

    public static void Queue(Action action) => UiApplyIncomingPacketServices.Queue(action);
}
