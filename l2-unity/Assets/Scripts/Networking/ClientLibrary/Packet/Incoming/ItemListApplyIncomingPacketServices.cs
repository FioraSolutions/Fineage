/// <summary>
/// Item list wire packet handler entry (#142 batch slice 145; combat hub slice 178).
/// Splits apply out of <see cref="CombatApplyIncomingPacketServices"/> (sub-slice 48).
/// Apply facade (#142 batch slice 154). Re-homes from <see cref="ItemListIncomingPacketServices"/>.
/// </summary>
public static class ItemListApplyIncomingPacketServices
{
    public static PlayerInventory Inventory => InventoryApplyIncomingPacketServices.Inventory;

    public static void ApplyItemList(ItemListDto packet) =>
        Inventory.SetInventory(
            packet.Items,
            packet.EquipItems,
            packet.ShowWindow,
            packet.AdenaCount,
            packet.Items.Count + packet.EquipItems.Count);
}
