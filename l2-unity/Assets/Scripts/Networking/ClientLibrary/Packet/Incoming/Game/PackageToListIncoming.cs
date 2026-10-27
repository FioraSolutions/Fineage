using UnityEngine;

[IncomingGamePacket(GameServerPacketType.PackageToList)]
public sealed class PackageToListIncoming : IncomingWirePacket<PackageToListDto>
{
    public override void Apply(PackageToListDto packet)
    {
        IncomingPacketServices.Queue(() =>
        {
            IncomingPacketServices.ShowList.AddList(packet.Players);
            IncomingPacketServices.ShowList.ShowWindow();
        });
    }
}
