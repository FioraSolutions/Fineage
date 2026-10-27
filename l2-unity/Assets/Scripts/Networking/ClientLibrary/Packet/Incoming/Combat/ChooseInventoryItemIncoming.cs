using UnityEngine;

[IncomingGamePacket(GameServerPacketType.ChooseInventoryItem)]
public sealed class ChooseInventoryItemIncoming : IncomingWirePacket<ChooseInventoryItemDto>
{
    public override void Apply(ChooseInventoryItemDto packet)
    {
        IncomingPacketServices.Queue(() => IncomingPacketServices.Enchant.ShowWindow(packet.Item));
    }
}
