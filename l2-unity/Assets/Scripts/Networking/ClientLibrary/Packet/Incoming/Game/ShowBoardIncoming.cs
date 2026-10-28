
[IncomingGamePacket(GameServerPacketType.ShowBoard)]
public sealed class ShowBoardIncoming : IncomingWirePacket<ShowCBoardDto>
{
    public override void Apply(ShowCBoardDto dto) =>
        ShowBoardApplyIncomingPacketServices.ApplyShowBoard(dto);
}
