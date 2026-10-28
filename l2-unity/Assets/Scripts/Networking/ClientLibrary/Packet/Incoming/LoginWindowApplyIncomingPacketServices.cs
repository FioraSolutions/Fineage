/// <summary>
/// Login window wire apply hub (#142 batch slice 170; login hub slice 176).
/// </summary>
public static class LoginWindowApplyIncomingPacketServices
{
    public static LoginClient Login => LobbyApplyIncomingPacketServices.Login;

    public static LoginWindow LoginWindow => LobbyApplyIncomingPacketServices.LoginWindow;
}
