/// <summary>
/// Inventory UI access with [Inject] wiring and live fallback (#142).
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
        _inventory != null ? _inventory : PlayerInventory.Instance;
}
