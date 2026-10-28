/// <summary>
/// Player inventory apply hub (#142 batch slice 162; combat packet hub slice 167).
/// </summary>
public static class InventoryApplyIncomingPacketServices
{
    public static PlayerInventory Inventory => CombatPacketApplyIncomingPacketServices.Inventory;
}
