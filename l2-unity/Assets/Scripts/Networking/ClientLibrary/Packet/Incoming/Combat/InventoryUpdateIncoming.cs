using UnityEngine;

[IncomingGamePacket(GameServerPacketType.InventoryUpdate)]
public sealed class InventoryUpdateIncoming : IncomingWirePacket<InventoryUpdateDto>
{
    public override void Apply(InventoryUpdateDto packet)
    {
        CombatIncomingPacketServices.Inventory.UpdateInventory(packet.Items, packet.EquipItems);
    }
}
