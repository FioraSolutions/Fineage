/// <summary>
/// LoginOk wire apply hook (#142 batch slice 146).
/// Splits apply out of <see cref="LoginOkIncomingPacketServices"/> (sub-slice 58).
/// </summary>
public static class LoginOkApplyIncomingPacketServices
{
    public static LoginClient Login => LocatorApplyIncomingPacketServices.Login;

    public static void ApplyLoginOk(LoginOkDto packet)
    {
        Login.OnAuthAllowed();
    }
}
