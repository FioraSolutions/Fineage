using UnityEngine;

[IncomingGamePacket(GameServerPacketType.PackageToList)]
public sealed class PackageToListIncoming : IncomingWirePacket<PackageToListDto>
{
    public override void Apply(PackageToListDto packet)
    {
        UiIncomingPacketServices.Queue(() =>
        {
            UiIncomingPacketServices.ShowList.AddList(packet.Players);
            UiIncomingPacketServices.ShowList.ShowWindow();
        });
    }
}
