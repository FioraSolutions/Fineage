using UnityEngine;

[IncomingGamePacket(GameServerPacketType.ChooseInventoryItem)]
public sealed class ChooseInventoryItemIncoming : IncomingWirePacket<ChooseInventoryItemDto>
{
    public override void Apply(ChooseInventoryItemDto packet)
    {
        CombatIncomingPacketServices.Queue(() => CombatIncomingPacketServices.Enchant.ShowWindow(packet.Item));
    }
}
