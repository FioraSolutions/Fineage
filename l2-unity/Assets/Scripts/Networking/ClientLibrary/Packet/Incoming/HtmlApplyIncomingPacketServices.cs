using System;

/// <summary>
/// HTML / community board apply hub (#142 batch slice 161).
/// </summary>
public static class HtmlApplyIncomingPacketServices
{
    public static HtmlWindow Html => UiApplyIncomingPacketServices.Html;

    public static void Queue(Action action) => IncomingPacketQueueApplyIncomingPacketServices.Queue(action);
}
