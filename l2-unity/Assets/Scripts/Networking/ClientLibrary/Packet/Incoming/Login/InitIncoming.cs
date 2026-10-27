[IncomingLoginPacket(LoginServerPacketType.Init)]
public sealed class InitIncoming : IncomingWirePacket<InitDto>
{
    protected override void OnParsed(InitDto packet)
    {
        IncomingPacketServices.Login.SetRSAKey(packet.PublicKey);
        IncomingPacketServices.Login.SetBlowFishKey(packet.BlowfishKey);
        IncomingPacketServices.Login.SetSessionId(packet.SessionId);
        IncomingPacketServices.Login.CompleteInitPacket();
        IncomingPacketServices.Login.Send(new AuthGameGuardCommand(packet.SessionId, packet.GG));
    }

    public override void Apply(InitDto packet)
    {
    }
}
