[IncomingLoginPacket(LoginServerPacketType.Init)]
public sealed class InitIncoming : IncomingWirePacket<InitDto>
{
    protected override void OnParsed(InitDto packet) =>
        InitIncomingPacketServices.OnInitParsed(packet);

    public override void Apply(InitDto packet) =>
        InitIncomingPacketServices.ApplyInit(packet);
}
