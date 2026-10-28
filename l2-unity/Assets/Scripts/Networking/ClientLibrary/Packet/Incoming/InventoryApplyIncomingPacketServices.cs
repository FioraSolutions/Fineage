/// <summary>
/// Player inventory apply hub (#142 batch slice 162; combat packet hub slice 167; queue slice 168).
/// </summary>
public static class InventoryApplyIncomingPacketServices
{
    public static PlayerInventory Inventory => CombatPacketApplyIncomingPacketServices.Inventory;

    public static void Queue(System.Action action) => CombatPacketApplyIncomingPacketServices.Queue(action);
}
