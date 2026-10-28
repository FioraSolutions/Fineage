/// <summary>
/// Player inventory apply hub (#142 batch slice 162; locator slice 171).
/// </summary>
public static class InventoryApplyIncomingPacketServices
{
    public static PlayerInventory Inventory => IncomingPacketLocatorApplyIncomingPacketServices.Inventory;

    public static void Queue(System.Action action) =>
        IncomingPacketQueueApplyIncomingPacketServices.Queue(action);
}
