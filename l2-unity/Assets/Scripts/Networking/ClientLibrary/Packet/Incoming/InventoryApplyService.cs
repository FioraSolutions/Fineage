/// <summary>
/// Inventory snapshot/update applies, constructed by the container instead of reached
/// through static facades (#142 slice 3).
/// </summary>
public sealed class InventoryApplyService
{
    public PlayerInventory Inventory => CombatPacketApplyIncomingPacketServices.Inventory;

    public void ApplyItemList(ItemListDto packet) =>
        Inventory.SetInventory(
            packet.Items,
            packet.EquipItems,
            packet.ShowWindow,
            packet.AdenaCount,
            packet.Items.Count + packet.EquipItems.Count);

    public void ApplyInventoryUpdate(InventoryUpdateDto packet) =>
        Inventory.UpdateInventory(packet.Items, packet.EquipItems);
}
