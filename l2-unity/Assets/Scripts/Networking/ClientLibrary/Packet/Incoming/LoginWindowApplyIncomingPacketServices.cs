/// <summary>
/// Login window wire apply hub (#142 batch slice 170; login hub slice 176).
/// </summary>
public static class LoginWindowApplyIncomingPacketServices
{
    public static LoginClient Login => LoginApplyIncomingPacketServices.Login;

    public static LoginWindow LoginWindow => LoginApplyIncomingPacketServices.LoginWindow;
}
