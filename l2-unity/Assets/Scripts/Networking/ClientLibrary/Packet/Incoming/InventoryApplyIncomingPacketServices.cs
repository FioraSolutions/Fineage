/// <summary>
/// Player inventory apply hub (#142 batch slice 162; session hub slice 179).
/// </summary>
public static class InventoryApplyIncomingPacketServices
{
    public static PlayerInventory Inventory => LocatorApplyIncomingPacketServices.Inventory;

    public static void Queue(System.Action action) =>
        CombatSessionApplyIncomingPacketServices.Queue(action);
}
