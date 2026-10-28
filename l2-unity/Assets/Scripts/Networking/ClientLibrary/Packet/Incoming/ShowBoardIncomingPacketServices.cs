/// <summary>
/// Community board HTML wire packet handler entry (#142 batch slice 140).
/// </summary>
public static class ShowBoardIncomingPacketServices
{
    public static void ApplyShowBoard(ShowCBoardDto dto)
    {
        if (IncomingPacketServices.GameWorld == null || !dto.ReadyToOpen)
            return;

        UiIncomingPacketServices.Html.InjectToCommunityWindow(dto.Html);
        UiIncomingPacketServices.Html.ToggleCommunityBoard(false);
    }
}
