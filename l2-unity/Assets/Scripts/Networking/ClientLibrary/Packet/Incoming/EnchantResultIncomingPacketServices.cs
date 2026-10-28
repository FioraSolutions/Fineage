using System;

/// <summary>
/// Enchant result wire packet handler entry (#142 batch slice 144).
/// </summary>
public static class EnchantResultIncomingPacketServices
{
    public static EnchantWindow Enchant => ItemEnchantPacketServices.Enchant;

    public static void Queue(Action action) => WorldApplyPacketServices.Queue(action);

    public static void ApplyEnchantResult(EnchantResultDto packet) =>
        Queue(() => Enchant.EnchantResult(packet.Result));
}
