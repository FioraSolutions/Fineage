[IncomingLoginPacket(LoginServerPacketType.PlayOk)]
public sealed class PlayOkIncoming : IncomingWirePacket<PlayOkDto>
{
    protected override void OnParsed(PlayOkDto packet) =>
        PlayOkParsedIncomingPacketServices.OnPlayOkParsed(packet);

    public override void Apply(PlayOkDto packet) =>
        PlayOkApplyIncomingPacketServices.ApplyPlayOk(packet);
}
