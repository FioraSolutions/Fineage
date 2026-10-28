using UnityEngine;

[IncomingGamePacket(GameServerPacketType.PackageSendableList)]
public sealed class PackageSendableListIncoming : IncomingWirePacket<PackageSendableListDto>
{
    public override void Apply(PackageSendableListDto packet) =>
        ShopListIncomingPacketServices.ApplyPackageSendableList(packet);
}
