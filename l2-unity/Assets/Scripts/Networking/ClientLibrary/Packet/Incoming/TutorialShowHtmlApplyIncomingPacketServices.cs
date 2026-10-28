using System;

/// <summary>
/// Tutorial HTML wire packet handler entry (#142 batch slice 140).
/// Apply facade (#142 batch slice 156). Re-homes from <see cref="TutorialShowHtmlIncomingPacketServices"/>.
/// </summary>
public static class TutorialShowUiApplyIncomingPacketServices
{
    public static void Queue(Action action) => UiApplyIncomingPacketServices.Queue(action);

    public static void ApplyTutorialShowHtml(TutorialShowHtmlDto packet) =>
        Queue(() =>
        {
            UiApplyIncomingPacketServices.Html.InjectToWindow(packet.Html);
            UiApplyIncomingPacketServices.Html.ShowWindowToCenter();
        });
}
