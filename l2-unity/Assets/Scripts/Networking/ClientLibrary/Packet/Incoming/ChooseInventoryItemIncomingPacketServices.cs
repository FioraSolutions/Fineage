using System;

/// <summary>
/// Choose-inventory-item wire packet handler entry (#142 batch slice 144).
/// </summary>
public static class ChooseInventoryItemIncomingPacketServices
{
    public static EnchantWindow Enchant => ItemEnchantPacketServices.Enchant;

    public static void Queue(Action action) => WorldApplyPacketServices.Queue(action);

    public static void ApplyChooseInventoryItem(ChooseInventoryItemDto packet) =>
        Queue(() => Enchant.ShowWindow(packet.Item));
}
