
[IncomingGamePacket(GameServerPacketType.PackageToList)]
public sealed class PackageToListIncoming : IncomingWirePacket<PackageToListDto>
{
    public override void Apply(PackageToListDto packet) => PackageToListIncomingPacketServices.ApplyPackageToList(packet);
}
