[IncomingLoginPacket(LoginServerPacketType.PlayOk)]
public sealed class PlayOkIncoming : IncomingWirePacket<PlayOkDto>
{
    protected override void OnParsed(PlayOkDto packet)
    {
        LoginServerIncomingPacketServices.OnPlayOkParsed(packet);
    }

    public override void Apply(PlayOkDto packet)
    {
        LoginServerIncomingPacketServices.ApplyPlayOk(packet);
    }
}
