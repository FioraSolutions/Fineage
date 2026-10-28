/// <summary>
/// Init wire packet handler entry (#142 sub-slice 107).
/// Splits init parse/apply out of <see cref="LoginServerIncomingPacketServices"/> (sub-slice 58 encapsulation).
/// </summary>
public static class InitIncomingPacketServices
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

    public static void ApplyInit(InitDto packet)
    {
    }
}
