/// <summary>
/// Client → zone: send a fresh ItemList. Empty body.
/// The opcode is not in the synced Net contract yet. It is the next free world id after
/// EnchantSkillInfo (0x03E0). Backend docs/inventory.md should use this same opcode.
/// </summary>
public static class InventoryResync
{
    public const ushort Opcode = 0x03E1;
}
