[IncomingLoginPacket(LoginServerPacketType.LoginOk)]
public sealed class LoginOkIncoming : IncomingWirePacket<LoginOkDto>
{
    protected override void OnParsed(LoginOkDto packet) =>
        LoginOkParsedIncomingPacketServices.OnLoginOkParsed(packet);

    public override void Apply(LoginOkDto packet) =>
        LoginOkApplyIncomingPacketServices.ApplyLoginOk(packet);
}
