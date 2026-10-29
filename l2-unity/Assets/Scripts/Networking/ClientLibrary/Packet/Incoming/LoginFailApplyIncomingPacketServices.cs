using System;

/// <summary>
/// LoginFail wire packet handler entry (#142 batch slice 147; login hub relay slice 257).
/// Re-homes login-fail apply from <see cref="LoginFailIncomingPacketServices"/>; splits from <see cref="LoginPacketServices"/> (sub-slice 58 encapsulation).
/// </summary>
public static class LoginFailApplyIncomingPacketServices
{
    public static LoginClient Login => LoginSessionApplyIncomingPacketServices.Login;

    public static LoginWindow LoginWindow => LoginSessionApplyIncomingPacketServices.LoginWindow;

    public static void Queue(Action action) => LoginSessionApplyIncomingPacketServices.Queue(action);

    public static void ApplyLoginFail(LoginFailDto packet) =>
        Queue(() =>
        {
            LoginWindow.ShowErrorText(packet.Message);
            Login.Disconnect();
        });
}
