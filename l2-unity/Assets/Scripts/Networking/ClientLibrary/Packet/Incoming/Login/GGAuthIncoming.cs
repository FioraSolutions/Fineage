[IncomingLoginPacket(LoginServerPacketType.GGAuth)]
public sealed class GGAuthIncoming : IncomingWirePacket<GGAuthDto>
{
    protected override void OnParsed(GGAuthDto packet) =>
        GGAuthParsedApplyIncomingPacketServices.OnGGAuthParsed(packet);

    public override void Apply(GGAuthDto packet) =>
        GGAuthApplyIncomingPacketServices.ApplyGGAuth(packet);
}
