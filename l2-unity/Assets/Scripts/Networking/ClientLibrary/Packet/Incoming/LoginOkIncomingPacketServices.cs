/// <summary>
/// LoginOk wire packet handler entry (#142 sub-slice 111).
/// Splits login-ok parse/apply out of <see cref="LoginServerIncomingPacketServices"/> (sub-slice 58 encapsulation).
/// </summary>
public static class LoginOkIncomingPacketServices
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

    public static void ApplyLoginOk(LoginOkDto packet)
    {
        Login.OnAuthAllowed();
    }
}
