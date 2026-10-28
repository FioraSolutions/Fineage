using System;

/// <summary>
/// ServerList wire packet handler entry (#142 batch slice 147; login hub relay slice 257).
/// Re-homes server-list apply from <see cref="ServerListIncomingPacketServices"/>; splits from <see cref="LoginPacketServices"/> (sub-slice 58 encapsulation).
/// </summary>
public static class ServerListApplyIncomingPacketServices
{
    public static LoginClient Login => LoginSessionApplyIncomingPacketServices.Login;

    public static void Queue(Action action) => LoginSessionApplyIncomingPacketServices.Queue(action);

    public static void ApplyServerList(ServerListDto packet) =>
        Queue(() => Login.OnServerListReceived(packet.LastServer, packet.ServersData, packet.CharsOnServers));
}
