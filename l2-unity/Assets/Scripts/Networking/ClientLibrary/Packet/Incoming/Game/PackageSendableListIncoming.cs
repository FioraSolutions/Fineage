
[IncomingGamePacket(GameServerPacketType.PackageSendableList)]
public sealed class PackageSendableListIncoming : IncomingWirePacket<PackageSendableListDto>
{
    public override void Apply(PackageSendableListDto packet) =>
        PackageSendableListIncomingPacketServices.ApplyPackageSendableList(packet);
}
