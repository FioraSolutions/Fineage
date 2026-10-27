using System;

/// <summary>
/// HTML / community-board wire packet handler access via UiIncomingPacketServices (#142 sub-slice 73).
/// </summary>
public static class HtmlIncomingPacketServices
{
    public static void Queue(Action action) => UiIncomingPacketServices.Queue(action);

    public static void ApplyShowBoard(ShowCBoardDto dto)
    {
        if (IncomingPacketServices.GameWorld == null || !dto.ReadyToOpen)
            return;

        UiIncomingPacketServices.Html.InjectToCommunityWindow(dto.Html);
        UiIncomingPacketServices.Html.ToggleCommunityBoard(false);
    }

    public static void ApplyTutorialShowHtml(TutorialShowHtmlDto packet) =>
        Queue(() =>
        {
            UiIncomingPacketServices.Html.InjectToWindow(packet.Html);
            UiIncomingPacketServices.Html.ShowWindowToCenter();
        });
}
