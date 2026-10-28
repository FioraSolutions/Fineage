[IncomingLoginPacket(LoginServerPacketType.Init)]
public sealed class InitIncoming : IncomingWirePacket<InitDto>
{
    protected override void OnParsed(InitDto packet) =>
        InitParsedApplyIncomingPacketServices.OnInitParsed(packet);

    public override void Apply(InitDto packet) =>
        InitApplyIncomingPacketServices.ApplyInit(packet);
}
