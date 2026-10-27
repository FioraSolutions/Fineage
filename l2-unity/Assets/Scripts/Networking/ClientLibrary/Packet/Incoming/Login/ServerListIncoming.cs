[IncomingLoginPacket(LoginServerPacketType.ServerList)]
public sealed class ServerListIncoming : IncomingWirePacket<ServerListDto>
{
    public override void Apply(ServerListDto packet)
    {
        LoginPacketServices.LoginClient.OnServerListReceived(packet.LastServer, packet.ServersData, packet.CharsOnServers);
    }
}
