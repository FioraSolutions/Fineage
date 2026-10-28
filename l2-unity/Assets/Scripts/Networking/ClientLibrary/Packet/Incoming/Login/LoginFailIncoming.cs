[IncomingLoginPacket(LoginServerPacketType.LoginFail)]
public sealed class LoginFailIncoming : IncomingWirePacket<LoginFailDto>
{
    public override void Apply(LoginFailDto packet) =>
        LoginFailIncomingPacketServices.ApplyLoginFail(packet);
}
