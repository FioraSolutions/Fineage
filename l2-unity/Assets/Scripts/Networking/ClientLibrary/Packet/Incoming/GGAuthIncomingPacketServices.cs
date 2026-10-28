/// <summary>
/// GGAuth wire packet handler entry (#142 sub-slice 108).
/// Splits GGAuth parse/apply out of <see cref="LoginServerIncomingPacketServices"/> (sub-slice 58 encapsulation).
/// </summary>
public static class GGAuthIncomingPacketServices
{
    public static LoginClient Login => LoginPacketServices.LoginClient;

    public static void OnGGAuthParsed(GGAuthDto packet)
    {
        LobbyFlowLog.Info("TX RequestAuthLogin account=" + (Login != null ? Login.Account : "?"));
        Login.Send(new RequestAuthLoginCommand(
            Login.Account,
            Login.Password,
            packet.Response));
    }

    public static void ApplyGGAuth(GGAuthDto packet)
    {
    }
}
