using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Shared UITK slot icon paint for inventory / trade / skill / buffer cells.
/// Prefer this over repeating IconManager + StyleBackground wiring per window.
/// </summary>
public static class L2SlotIconView
{
    public static void SetItemIcon(VisualElement target, int itemId)
    {
        if (target == null || IconManager.Instance == null)
            return;

        target.style.backgroundImage = new StyleBackground(IconManager.Instance.GetIcon(itemId));
    }

    public static void SetItemIconAtIndex(VisualElement target, int itemId, int iconIndex)
    {
        if (target == null || IconManager.Instance == null)
            return;

        target.style.backgroundImage = new StyleBackground(
            IconManager.Instance.GetOtherIcon(itemId, iconIndex));
    }

    public static void SetSkillIcon(VisualElement target, string iconName)
    {
        if (target == null || IconManager.Instance == null || string.IsNullOrEmpty(iconName))
            return;

        Texture2D icon = IconManager.Instance.LoadTextureByName(iconName);
        if (icon != null)
            target.style.backgroundImage = icon;
    }

    public static void SetEmptyInventoryBackground(VisualElement target)
    {
        if (target == null || IconManager.Instance == null)
            return;

        target.style.backgroundImage = new StyleBackground(
            IconManager.Instance.GetInvetoryDefaultBackground());
    }

    public static void Clear(VisualElement target)
    {
        if (target == null)
            return;

        target.style.backgroundImage = null;
    }

    /// <summary>
    /// Inventory/gear slots paint GearSlot chrome on the root element; everyone else uses SlotBg.
    /// </summary>
    public static void SetInventoryStyleItemIcon(
        VisualElement slotBg,
        VisualElement slotElement,
        int itemId,
        bool paintOnRootElement)
    {
        if (paintOnRootElement)
            SetItemIcon(slotElement, itemId);
        else if (slotBg != null)
            SetItemIcon(slotBg, itemId);
    }

    public static void SetInventoryStyleItemIconAtIndex(
        VisualElement slotBg,
        VisualElement slotElement,
        int itemId,
        int iconIndex,
        bool paintOnRootElement)
    {
        if (paintOnRootElement)
            SetItemIconAtIndex(slotElement, itemId, iconIndex);
        else if (slotBg != null)
            SetItemIconAtIndex(slotBg, itemId, iconIndex);
    }
}
