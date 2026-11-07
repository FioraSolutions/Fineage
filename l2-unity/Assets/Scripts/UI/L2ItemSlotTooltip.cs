/// <summary>
/// Shared tooltip label for item slots (inventory / player trade / shop).
/// Equipment categories omit the stack count, matching Interlude chrome.
/// </summary>
public static class L2ItemSlotTooltip
{
    public static string Format(string name, int count, ItemCategory category)
    {
        if (category == ItemCategory.Weapon ||
            category == ItemCategory.Jewel ||
            category == ItemCategory.ShieldArmor)
        {
            return name;
        }

        return $"{name} ({count})";
    }
}
