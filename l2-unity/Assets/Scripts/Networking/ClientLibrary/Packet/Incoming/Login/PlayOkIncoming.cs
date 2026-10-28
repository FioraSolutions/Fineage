[IncomingLoginPacket(LoginServerPacketType.PlayOk)]
public sealed class PlayOkIncoming : IncomingWirePacket<PlayOkDto>
{
    protected override void OnParsed(PlayOkDto packet) =>
        PlayOkIncomingPacketServices.OnPlayOkParsed(packet);

    public override void Apply(PlayOkDto packet) =>
        PlayOkIncomingPacketServices.ApplyPlayOk(packet);
}
