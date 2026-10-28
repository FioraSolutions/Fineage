/// <summary>
/// LoginFail wire packet handler entry (#142 sub-slice 109).
/// Splits login-fail apply out of <see cref="LoginServerIncomingPacketServices"/> (sub-slice 58 encapsulation).
/// </summary>
public static class LoginFailIncomingPacketServices
{
    public static LoginClient Login => LoginPacketServices.LoginClient;

    public static LoginWindow LoginWindow => LoginPacketServices.LoginWindow;

    public static void ApplyLoginFail(LoginFailDto packet)
    {
        LoginWindow.ShowErrorText(packet.Message);
        Login.Disconnect();
    }
}
