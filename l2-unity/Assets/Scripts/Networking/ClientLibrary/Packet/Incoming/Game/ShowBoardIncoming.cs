using UnityEngine;

[IncomingGamePacket(GameServerPacketType.ShowBoard)]
public sealed class ShowBoardIncoming : IncomingWirePacket<ShowCBoardDto>
{
    public override void Apply(ShowCBoardDto dto)
    {
        if (IncomingPacketServices.GameWorld == null || !dto.ReadyToOpen)
            return;

        UiIncomingPacketServices.Html.InjectToCommunityWindow(dto.Html);
        UiIncomingPacketServices.Html.ToggleCommunityBoard(false);
    }
}
