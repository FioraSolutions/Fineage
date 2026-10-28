using System;

/// <summary>
/// Tutorial HTML wire packet handler entry (#142 batch slice 140).
/// Apply facade (#142 batch slice 156). Re-homes from <see cref="TutorialShowHtmlIncomingPacketServices"/>.
/// </summary>
public static class TutorialShowHtmlApplyIncomingPacketServices
{
    public static void Queue(Action action) => HtmlApplyIncomingPacketServices.Queue(action);

    public static void ApplyTutorialShowHtml(TutorialShowHtmlDto packet) =>
        Queue(() =>
        {
            HtmlApplyIncomingPacketServices.Html.InjectToWindow(packet.Html);
            HtmlApplyIncomingPacketServices.Html.ShowWindowToCenter();
        });
}
