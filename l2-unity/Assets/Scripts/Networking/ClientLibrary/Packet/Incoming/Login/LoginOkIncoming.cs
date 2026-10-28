[IncomingLoginPacket(LoginServerPacketType.LoginOk)]
public sealed class LoginOkIncoming : IncomingWirePacket<LoginOkDto>
{
    protected override void OnParsed(LoginOkDto packet) =>
        LoginOkIncomingPacketServices.OnLoginOkParsed(packet);

    public override void Apply(LoginOkDto packet) =>
        LoginOkIncomingPacketServices.ApplyLoginOk(packet);
}
