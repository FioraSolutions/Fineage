/// <summary>
/// Inventory UI access to game services with [Inject] wiring and IncomingPacketActions fallback (#142 sub-slice 20).
/// </summary>
public static class InventoryPacketServices
{
    private static PlayerInventory _inventory;

    public static void Bind(PlayerInventory inventory = null)
    {
        if (inventory != null)
            _inventory = inventory;
    }

    public static PlayerInventory Inventory =>
        _inventory != null ? _inventory : IncomingPacketActions.Inventory;
}
