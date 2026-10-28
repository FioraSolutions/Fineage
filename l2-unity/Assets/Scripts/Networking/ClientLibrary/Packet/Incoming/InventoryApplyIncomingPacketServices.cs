/// <summary>
/// Player inventory apply hub (#142 batch slice 162; combat world slice 176).
/// </summary>
public static class InventoryApplyIncomingPacketServices
{
    public static PlayerInventory Inventory => LocatorApplyIncomingPacketServices.Inventory;

    public static void Queue(System.Action action) =>
        CombatWorldApplyIncomingPacketServices.Queue(action);
}
