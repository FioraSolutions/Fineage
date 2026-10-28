/// <summary>
/// LoginOk wire parse hook (#142 batch slice 146).
/// Splits parse out of <see cref="LoginOkIncomingPacketServices"/> (sub-slice 58).
/// </summary>
public static class LoginOkParsedIncomingPacketServices
{
    public static LoginClient Login => LoginPacketServices.LoginClient;

    public static GameClient Game => LoginPacketServices.Game;

    public static void OnLoginOkParsed(LoginOkDto packet)
    {
        Login.SessionKey1 = packet.SessionKey1;
        Login.SessionKey2 = packet.SessionKey2;
        Game.SessionKey1 = packet.SessionKey1;
        Game.SessionKey2 = packet.SessionKey2;
    }
}
