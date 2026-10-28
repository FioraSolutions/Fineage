/// <summary>
/// GGAuth wire parse hook (#142 batch slice 146; login hub slice 177).
/// Splits parse out of <see cref="GGAuthIncomingPacketServices"/> (sub-slice 58).
/// </summary>
public static class GGAuthParsedApplyIncomingPacketServices
{
    public static LoginClient Login => LocatorApplyIncomingPacketServices.Login;

    public static void OnGGAuthParsed(GGAuthDto packet)
    {
        LobbyFlowLog.Info("TX RequestAuthLogin account=" + (Login != null ? Login.Account : "?"));
        Login.Send(new RequestAuthLoginCommand(
            Login.Account,
            Login.Password,
            packet.Response));
    }
}
