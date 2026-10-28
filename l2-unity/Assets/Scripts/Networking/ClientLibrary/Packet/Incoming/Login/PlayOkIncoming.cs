[IncomingLoginPacket(LoginServerPacketType.PlayOk)]
public sealed class PlayOkIncoming : IncomingWirePacket<PlayOkDto>
{
    protected override void OnParsed(PlayOkDto packet) =>
        PlayOkParsedApplyIncomingPacketServices.OnPlayOkParsed(packet);

    public override void Apply(PlayOkDto packet) =>
        PlayOkApplyIncomingPacketServices.ApplyPlayOk(packet);
}
