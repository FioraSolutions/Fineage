using System;

/// <summary>
/// LoginOk wire apply hook (#142 batch slice 146; login hub relay slice 257).
/// Splits apply out of <see cref="LoginOkIncomingPacketServices"/> (sub-slice 58).
/// </summary>
public static class LoginOkApplyIncomingPacketServices
{
    public static LoginClient Login => LoginSessionApplyIncomingPacketServices.Login;

    public static void Queue(Action action) => LoginSessionApplyIncomingPacketServices.Queue(action);

    public static void ApplyLoginOk(LoginOkDto packet) =>
        Queue(() => Login.OnAuthAllowed());
}
