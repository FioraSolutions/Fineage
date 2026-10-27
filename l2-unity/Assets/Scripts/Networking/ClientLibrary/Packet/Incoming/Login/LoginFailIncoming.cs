[IncomingLoginPacket(LoginServerPacketType.LoginFail)]
public sealed class LoginFailIncoming : IncomingWirePacket<LoginFailDto>
{
    public override void Apply(LoginFailDto packet)
    {
        LoginPacketServices.LoginWindow.ShowErrorText(packet.Message);
        LoginPacketServices.LoginClient.Disconnect();
    }
}
