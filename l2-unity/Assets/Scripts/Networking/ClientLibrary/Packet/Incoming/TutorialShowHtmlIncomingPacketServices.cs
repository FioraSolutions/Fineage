using System;

/// <summary>
/// Tutorial HTML wire packet handler entry (#142 batch slice 140).
/// </summary>
public static class TutorialShowHtmlIncomingPacketServices
{
    public static void Queue(Action action) => UiIncomingPacketServices.Queue(action);

    public static void ApplyTutorialShowHtml(TutorialShowHtmlDto packet) =>
        Queue(() =>
        {
            UiIncomingPacketServices.Html.InjectToWindow(packet.Html);
            UiIncomingPacketServices.Html.ShowWindowToCenter();
        });
}
