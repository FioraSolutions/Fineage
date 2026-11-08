/// <summary>Canonical body paperdoll slots used by the armor dresser.</summary>
public static class ArmorBodySlots
{
    public static ItemSlot Normalize(ItemSlot slot)
    {
        return slot == ItemSlot.boots ? ItemSlot.feet : slot;
    }

    public static bool IsBody(ItemSlot slot)
    {
        slot = Normalize(slot);
        return slot == ItemSlot.fullarmor
            || slot == ItemSlot.chest
            || slot == ItemSlot.legs
            || slot == ItemSlot.gloves
            || slot == ItemSlot.feet;
    }
}
