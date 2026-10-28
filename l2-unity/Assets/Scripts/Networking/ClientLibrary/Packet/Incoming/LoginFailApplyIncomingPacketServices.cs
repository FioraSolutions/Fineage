/// <summary>
/// LoginFail wire packet handler entry (#142 batch slice 147).
/// Re-homes login-fail apply from <see cref="LoginFailIncomingPacketServices"/>; splits from <see cref="LoginPacketServices"/> (sub-slice 58 encapsulation).
/// </summary>
public static class LoginFailApplyIncomingPacketServices
{
    public static LoginClient Login => LoginWindowApplyIncomingPacketServices.Login;

    public static LoginWindow LoginWindow => LoginWindowApplyIncomingPacketServices.LoginWindow;

    public static void ApplyLoginFail(LoginFailDto packet)
    {
        LoginWindow.ShowErrorText(packet.Message);
        Login.Disconnect();
    }
}
