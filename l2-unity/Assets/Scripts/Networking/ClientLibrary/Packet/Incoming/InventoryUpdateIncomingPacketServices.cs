/// <summary>
/// Inventory update wire packet handler entry (#142 batch slice 145).
/// Splits apply out of <see cref="CombatIncomingPacketServices"/> (sub-slice 48).
/// </summary>
public static class InventoryUpdateIncomingPacketServices
{
    public static PlayerInventory Inventory => InventoryPacketServices.Inventory;

    public static void ApplyInventoryUpdate(InventoryUpdateDto packet) =>
        Inventory.UpdateInventory(packet.Items, packet.EquipItems);
}
