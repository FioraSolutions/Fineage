[IncomingLoginPacket(LoginServerPacketType.LoginOk)]
public sealed class LoginOkIncoming : IncomingWirePacket<LoginOkDto>
{
    protected override void OnParsed(LoginOkDto packet)
    {
        LoginPacketServices.LoginClient.SessionKey1 = packet.SessionKey1;
        LoginPacketServices.LoginClient.SessionKey2 = packet.SessionKey2;
        LoginPacketServices.Game.SessionKey1 = packet.SessionKey1;
        LoginPacketServices.Game.SessionKey2 = packet.SessionKey2;
    }

    public override void Apply(LoginOkDto packet)
    {
        LoginPacketServices.LoginClient.OnAuthAllowed();
    }
}
