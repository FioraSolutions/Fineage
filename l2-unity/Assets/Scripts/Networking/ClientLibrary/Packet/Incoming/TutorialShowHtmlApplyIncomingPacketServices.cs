using System;

/// <summary>
/// Tutorial HTML wire packet handler entry (#142 batch slice 140).
/// Apply facade (#142 batch slice 156; locator queue slice 259). Re-homes from <see cref="TutorialShowHtmlIncomingPacketServices"/>.
/// </summary>
public static class TutorialShowHtmlApplyIncomingPacketServices
{
    public static HtmlWindow Html => LocatorApplyIncomingPacketServices.Html;

    public static void Queue(Action action) => QueueApplyIncomingPacketServices.Queue(action);

    public static void ApplyTutorialShowHtml(TutorialShowHtmlDto packet) =>
        Queue(() =>
        {
            Html.InjectToWindow(packet.Html);
            Html.ShowWindowToCenter();
        });
}
