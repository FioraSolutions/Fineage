/// <summary>
/// Inventory update wire packet handler entry (#142 batch slice 145; combat hub slice 178).
/// Splits apply out of <see cref="CombatApplyIncomingPacketServices"/> (sub-slice 48).
/// Apply facade (#142 batch slice 154). Re-homes from <see cref="InventoryUpdateIncomingPacketServices"/>.
/// </summary>
public static class InventoryUpdateApplyIncomingPacketServices
{
    public static PlayerInventory Inventory => CombatPacketApplyIncomingPacketServices.Inventory;

    public static void ApplyInventoryUpdate(InventoryUpdateDto packet) =>
        Inventory.UpdateInventory(packet.Items, packet.EquipItems);
}
