/// <summary>
/// ServerList wire packet handler entry (#142 sub-slice 110).
/// Splits server-list apply out of <see cref="LoginServerIncomingPacketServices"/> (sub-slice 58 encapsulation).
/// </summary>
public static class ServerListIncomingPacketServices
{
    public static LoginClient Login => LoginPacketServices.LoginClient;

    public static void ApplyServerList(ServerListDto packet)
    {
        Login.OnServerListReceived(packet.LastServer, packet.ServersData, packet.CharsOnServers);
    }
}
