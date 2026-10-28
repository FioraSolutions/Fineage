/// <summary>
/// Community board HTML wire packet handler entry (#142 batch slice 140; world hub slice 179).
/// Apply facade (#142 batch slice 156). Re-homes from <see cref="ShowBoardIncomingPacketServices"/>.
/// </summary>
public static class ShowBoardApplyIncomingPacketServices
{
    public static void ApplyShowBoard(ShowCBoardDto dto)
    {
        if (WorldApplyIncomingPacketServices.GameWorld == null || !dto.ReadyToOpen)
            return;

        UiApplyIncomingPacketServices.Html.InjectToCommunityWindow(dto.Html);
        UiApplyIncomingPacketServices.Html.ToggleCommunityBoard(false);
    }
}
