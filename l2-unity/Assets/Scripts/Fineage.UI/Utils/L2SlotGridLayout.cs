using UnityEngine.UIElements;

/// <summary>
/// Shared UITK slot-grid sizing and instantiate helpers for inventory / dealer / trading tabs.
/// </summary>
public static class L2SlotGridLayout
{
    public const int DefaultMerchantRowSize = 6;
    public const int DefaultMerchantMinimumSlots = 24;

    /// <summary>
    /// Grow to at least <paramref name="minimumSlots"/>, then pad up to a multiple of
    /// <paramref name="rowSize"/> (Interlude merchant grids are 6-wide).
    /// </summary>
    public static int CapacityForItemCount(
        int itemCount,
        int minimumSlots = DefaultMerchantMinimumSlots,
        int rowSize = DefaultMerchantRowSize)
    {
        if (itemCount <= minimumSlots)
            return minimumSlots;

        if (rowSize <= 0)
            return itemCount;

        int remainder = itemCount % rowSize;
        return remainder == 0 ? itemCount : itemCount + (rowSize - remainder);
    }

    public static VisualElement InstantiateSlot(VisualTreeAsset template, VisualElement parent)
    {
        VisualElement slotElement = template.Instantiate()[0];
        if (parent != null)
            parent.Add(slotElement);
        return slotElement;
    }
}
