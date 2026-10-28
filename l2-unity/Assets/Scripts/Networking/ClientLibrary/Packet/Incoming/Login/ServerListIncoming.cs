[IncomingLoginPacket(LoginServerPacketType.ServerList)]
public sealed class ServerListIncoming : IncomingWirePacket<ServerListDto>
{
    public override void Apply(ServerListDto packet) =>
        ServerListIncomingPacketServices.ApplyServerList(packet);
}
