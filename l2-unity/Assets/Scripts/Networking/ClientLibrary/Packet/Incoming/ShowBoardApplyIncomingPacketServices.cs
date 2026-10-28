/// <summary>
/// Community board HTML wire packet handler entry (#142 batch slice 140; world hub slice 179).
/// Apply facade (#142 batch slice 156). Re-homes from <see cref="ShowBoardIncomingPacketServices"/>.
/// </summary>
public static class ShowBoardApplyIncomingPacketServices
{
    public static void ApplyShowBoard(ShowCBoardDto dto)
    {
        if (WorldEffectApplyIncomingPacketServices.GameWorld == null || !dto.ReadyToOpen)
            return;

        HtmlApplyIncomingPacketServices.Html.InjectToCommunityWindow(dto.Html);
        HtmlApplyIncomingPacketServices.Html.ToggleCommunityBoard(false);
    }
}
