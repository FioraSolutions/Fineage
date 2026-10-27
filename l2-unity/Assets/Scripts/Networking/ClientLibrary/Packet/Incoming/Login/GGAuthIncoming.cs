[IncomingLoginPacket(LoginServerPacketType.GGAuth)]
public sealed class GGAuthIncoming : IncomingWirePacket<GGAuthDto>
{
    protected override void OnParsed(GGAuthDto packet)
    {
        LoginClient login = IncomingPacketServices.Login;
        LobbyFlowLog.Info("TX RequestAuthLogin account=" + (login != null ? login.Account : "?"));
        login.Send(new RequestAuthLoginCommand(
            login.Account,
            login.Password,
            packet.Response));
    }

    public override void Apply(GGAuthDto packet)
    {
    }
}
