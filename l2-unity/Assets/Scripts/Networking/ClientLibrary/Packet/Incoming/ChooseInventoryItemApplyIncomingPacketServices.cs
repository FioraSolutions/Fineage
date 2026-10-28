using System;

/// <summary>
/// Choose-inventory-item wire packet handler entry (#142 batch slice 144).
/// Apply facade (#142 batch slice 156). Re-homes from <see cref="ChooseInventoryItemIncomingPacketServices"/>.
/// </summary>
public static class ChooseInventoryItemApplyIncomingPacketServices
{
    public static EnchantWindow Enchant => EnchantApplyIncomingPacketServices.Enchant;

    public static void Queue(Action action) => EnchantApplyIncomingPacketServices.Queue(action);

    public static void ApplyChooseInventoryItem(ChooseInventoryItemDto packet) =>
        Queue(() => Enchant.ShowWindow(packet.Item));
}
