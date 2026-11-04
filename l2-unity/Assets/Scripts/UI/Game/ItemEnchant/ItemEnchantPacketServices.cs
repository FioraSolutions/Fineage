/// <summary>
/// Item enchant UI access to game services with [Inject] wiring and IncomingPacketActions fallback (#142 sub-slice 23).
/// </summary>
public static class ItemEnchantPacketServices
{
    private static EnchantWindow _enchant;

    public static void Bind(EnchantWindow enchant = null)
    {
        if (enchant != null)
            _enchant = enchant;
    }

    public static EnchantWindow Enchant =>
        _enchant != null ? _enchant : IncomingPacketActions.Enchant;
}
