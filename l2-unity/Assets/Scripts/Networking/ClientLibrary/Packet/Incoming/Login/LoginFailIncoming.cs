[IncomingLoginPacket(LoginServerPacketType.LoginFail)]
public sealed class LoginFailIncoming : IncomingWirePacket<LoginFailDto>
{
    public override void Apply(LoginFailDto packet)
    {
        IncomingPacketServices.LoginWindow.ShowErrorText(packet.Message);
        IncomingPacketServices.Login.Disconnect();
    }
}
