[IncomingLoginPacket(LoginServerPacketType.Init)]
public sealed class InitIncoming : IncomingWirePacket<InitDto>
{
    protected override void OnParsed(InitDto packet)
    {
        LoginServerIncomingPacketServices.OnInitParsed(packet);
    }

    public override void Apply(InitDto packet) =>
        LoginServerIncomingPacketServices.ApplyInit(packet);
}
