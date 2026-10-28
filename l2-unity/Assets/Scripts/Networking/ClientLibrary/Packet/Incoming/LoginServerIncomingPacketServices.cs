/// <summary>
/// Login-server wire packet handler access via LoginPacketServices (#142 sub-slice 58).
/// </summary>
public static class LoginServerIncomingPacketServices
{
    public static LoginClient Login => LoginPacketServices.LoginClient;

    public static GameClient Game => LoginPacketServices.Game;

    public static GameManager Manager => LoginPacketServices.Manager;

    public static LoginWindow LoginWindow => LoginPacketServices.LoginWindow;

}
