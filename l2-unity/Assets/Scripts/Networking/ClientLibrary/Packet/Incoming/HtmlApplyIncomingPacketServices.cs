using System;

/// <summary>
/// HTML / community board apply hub (#142 batch slice 161; UI hub slice 167).
/// </summary>
public static class HtmlApplyIncomingPacketServices
{
    public static HtmlWindow Html => UiApplyIncomingPacketServices.Html;

    public static void Queue(Action action) => UiApplyIncomingPacketServices.Queue(action);
}
