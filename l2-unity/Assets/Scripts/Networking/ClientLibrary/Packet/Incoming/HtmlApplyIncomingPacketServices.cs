using System;

/// <summary>
/// HTML / community board apply hub (#142 batch slice 161; locator slice 170).
/// </summary>
public static class HtmlApplyIncomingPacketServices
{
    public static HtmlWindow Html => IncomingPacketLocatorApplyIncomingPacketServices.Html;

    public static void Queue(Action action) =>
        QueueApplyIncomingPacketServices.Queue(action);
}
