/// <summary>
/// Item list wire packet handler entry (#142 batch slice 145).
/// Splits apply out of <see cref="CombatIncomingPacketServices"/> (sub-slice 48).
/// </summary>
public static class ItemListIncomingPacketServices
{
    public static PlayerInventory Inventory => InventoryPacketServices.Inventory;

    public static void ApplyItemList(ItemListDto packet) =>
        Inventory.SetInventory(
            packet.Items,
            packet.EquipItems,
            packet.ShowWindow,
            packet.AdenaCount,
            packet.Items.Count + packet.EquipItems.Count);
}
