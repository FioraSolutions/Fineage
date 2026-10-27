/// <summary>
/// InterludeKey / AuthLogin wire path incoming handler access via LoginPacketServices (#142 sub-slice 53).
/// </summary>
public static class LobbyWireIncomingPacketServices
{
    public static LoginClient Login => LoginPacketServices.LoginClient;

    public static GameClient Game => LoginPacketServices.Game;
}
