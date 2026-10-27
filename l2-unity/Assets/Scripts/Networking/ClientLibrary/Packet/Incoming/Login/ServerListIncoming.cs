[IncomingLoginPacket(LoginServerPacketType.ServerList)]
public sealed class ServerListIncoming : IncomingWirePacket<ServerListDto>
{
    public override void Apply(ServerListDto packet)
    {
        IncomingPacketServices.Login.OnServerListReceived(packet.LastServer, packet.ServersData, packet.CharsOnServers);
    }
}
