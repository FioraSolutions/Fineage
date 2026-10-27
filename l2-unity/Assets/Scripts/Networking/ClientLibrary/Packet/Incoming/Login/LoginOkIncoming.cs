[IncomingLoginPacket(LoginServerPacketType.LoginOk)]
public sealed class LoginOkIncoming : IncomingWirePacket<LoginOkDto>
{
    protected override void OnParsed(LoginOkDto packet)
    {
        IncomingPacketServices.Login.SessionKey1 = packet.SessionKey1;
        IncomingPacketServices.Login.SessionKey2 = packet.SessionKey2;
        IncomingPacketServices.Game.SessionKey1 = packet.SessionKey1;
        IncomingPacketServices.Game.SessionKey2 = packet.SessionKey2;
    }

    public override void Apply(LoginOkDto packet)
    {
        IncomingPacketServices.Login.OnAuthAllowed();
    }
}
