[IncomingLoginPacket(LoginServerPacketType.Init)]
public sealed class InitIncoming : IncomingWirePacket<InitDto>
{
    protected override void OnParsed(InitDto packet)
    {
        LoginPacketServices.LoginClient.SetRSAKey(packet.PublicKey);
        LoginPacketServices.LoginClient.SetBlowFishKey(packet.BlowfishKey);
        LoginPacketServices.LoginClient.SetSessionId(packet.SessionId);
        LoginPacketServices.LoginClient.CompleteInitPacket();
        LoginPacketServices.LoginClient.Send(new AuthGameGuardCommand(packet.SessionId, packet.GG));
    }

    public override void Apply(InitDto packet)
    {
    }
}
