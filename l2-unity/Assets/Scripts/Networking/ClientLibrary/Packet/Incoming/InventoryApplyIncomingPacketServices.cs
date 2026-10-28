/// <summary>
/// Player inventory apply hub (#142 batch slice 162; locator backing slice 165).
/// </summary>
public static class InventoryApplyIncomingPacketServices
{
    public static PlayerInventory Inventory => IncomingPacketLocatorApplyIncomingPacketServices.Inventory;
}
