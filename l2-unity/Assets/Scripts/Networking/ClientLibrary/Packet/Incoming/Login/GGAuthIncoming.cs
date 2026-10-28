[IncomingLoginPacket(LoginServerPacketType.GGAuth)]
public sealed class GGAuthIncoming : IncomingWirePacket<GGAuthDto>
{
    protected override void OnParsed(GGAuthDto packet) =>
        GGAuthIncomingPacketServices.OnGGAuthParsed(packet);

    public override void Apply(GGAuthDto packet) =>
        GGAuthIncomingPacketServices.ApplyGGAuth(packet);
}
