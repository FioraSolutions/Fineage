using System;

/// <summary>
/// HTML / community board apply hub (#142 batch slice 161; UI leaf slice 180).
/// </summary>
public static class HtmlApplyIncomingPacketServices
{
    public static HtmlWindow Html => LocatorApplyIncomingPacketServices.Html;

    public static void Queue(Action action) =>
        UiApplyIncomingPacketServices.Queue(action);
}
