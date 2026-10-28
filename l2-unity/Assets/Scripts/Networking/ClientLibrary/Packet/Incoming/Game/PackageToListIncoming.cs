
[IncomingGamePacket(GameServerPacketType.PackageToList)]
public sealed class PackageToListIncoming : IncomingWirePacket<PackageToListDto>
{
    public override void Apply(PackageToListDto packet) => PackageToListApplyIncomingPacketServices.ApplyPackageToList(packet);
}
