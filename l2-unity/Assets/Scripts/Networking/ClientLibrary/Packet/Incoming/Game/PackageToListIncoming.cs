
[IncomingGamePacket(GameServerPacketType.PackageToList)]
public sealed class PackageToListIncoming : IncomingWirePacket<PackageToListDto>
{
    public override void Apply(PackageToListDto packet) => ShowListIncomingPacketServices.ApplyPackageToList(packet);
}
