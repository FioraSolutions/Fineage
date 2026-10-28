/// <summary>
/// Init wire parse hook (#142 batch slice 146).
/// Splits parse out of <see cref="InitIncomingPacketServices"/> (sub-slice 58).
/// </summary>
public static class InitParsedIncomingPacketServices
{
    public static LoginClient Login => LoginPacketServices.LoginClient;

    public static void OnInitParsed(InitDto packet)
    {
        Login.SetRSAKey(packet.PublicKey);
        Login.SetBlowFishKey(packet.BlowfishKey);
        Login.SetSessionId(packet.SessionId);
        Login.CompleteInitPacket();
        Login.Send(new AuthGameGuardCommand(packet.SessionId, packet.GG));
    }
}
