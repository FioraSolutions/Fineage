using System;

/// <summary>
/// Enchant result wire packet handler entry (#142 batch slice 144; combat hub slice 178).
/// Apply facade (#142 batch slice 154). Re-homes from <see cref="EnchantResultIncomingPacketServices"/>.
/// </summary>
public static class EnchantResultApplyIncomingPacketServices
{
    public static EnchantWindow Enchant => CombatPacketApplyIncomingPacketServices.Enchant;

    public static void Queue(Action action) => CombatPacketApplyIncomingPacketServices.Queue(action);

    public static void ApplyEnchantResult(EnchantResultDto packet) =>
        Queue(() => Enchant.EnchantResult(packet.Result));
}
