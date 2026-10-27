/// <summary>
/// Skillbar UI access to game services with [Inject] wiring and IncomingPacketActions fallback (#142 sub-slice 27).
/// </summary>
public static class SkillbarPacketServices
{
    private static SkillbarWindow _skillbar;
    private static PlayerShortcuts _shortcuts;
    private static PlayerInventory _inventory;

    public static void Bind(
        SkillbarWindow skillbar = null,
        PlayerShortcuts shortcuts = null,
        PlayerInventory inventory = null)
    {
        if (skillbar != null)
            _skillbar = skillbar;
        if (shortcuts != null)
            _shortcuts = shortcuts;
        if (inventory != null)
            _inventory = inventory;
    }

    public static SkillbarWindow Skillbar =>
        _skillbar != null ? _skillbar : IncomingPacketActions.Skillbar;

    public static PlayerShortcuts Shortcuts =>
        _shortcuts != null ? _shortcuts : IncomingPacketActions.Shortcuts;

    public static PlayerInventory Inventory =>
        _inventory != null ? _inventory : IncomingPacketActions.Inventory;
}
