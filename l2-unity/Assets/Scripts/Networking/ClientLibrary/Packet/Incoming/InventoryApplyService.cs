using VContainer;

/// <summary>
/// Inventory snapshot/update applies, constructed by the container instead of reached
/// through static facades (#142 slice 3). <see cref="PlayerInventory"/> lives in UI.prefab
/// and appears after the scope, so it is read through <see cref="ILateBound{T}"/> on every
/// call rather than captured once (#142 slice 4).
/// </summary>
public sealed class InventoryApplyService
{
    private readonly ILateBound<PlayerInventory> _inventory;

    [Inject]
    public InventoryApplyService(ILateBound<PlayerInventory> inventory)
    {
        _inventory = inventory;
    }

    public PlayerInventory Inventory => _inventory != null ? _inventory.Value : null;

    public void ApplyItemList(ItemListDto packet)
    {
        PlayerInventory inventory = Inventory;
        if (inventory == null)
            return;

        inventory.SetInventory(
            packet.Items,
            packet.EquipItems,
            packet.ShowWindow,
            packet.AdenaCount,
            packet.Items.Count + packet.EquipItems.Count);
    }

    public void ApplyInventoryUpdate(InventoryUpdateDto packet)
    {
        PlayerInventory inventory = Inventory;
        if (inventory == null)
            return;

        inventory.UpdateInventory(packet.Items, packet.EquipItems);
    }
}
