/// <summary>
/// ServerList wire packet handler entry (#142 batch slice 147).
/// Re-homes server-list apply from <see cref="ServerListIncomingPacketServices"/>; splits from <see cref="LoginPacketServices"/> (sub-slice 58 encapsulation).
/// </summary>
public static class ServerListApplyIncomingPacketServices
{
    public static LoginClient Login => LoginParsedApplyIncomingPacketServices.Login;

    public static void ApplyServerList(ServerListDto packet)
    {
        Login.OnServerListReceived(packet.LastServer, packet.ServersData, packet.CharsOnServers);
    }
}
