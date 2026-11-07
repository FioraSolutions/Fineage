using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Shared UITK slot/chrome icon paint for inventory, trade, skill, buffer, and DF UI.
/// Prefer this over repeating IconManager / Resources.Load + StyleBackground per window.
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

    /// <summary>Icon atlas entry (skills, actions, item icon names).</summary>
    public static void SetNamedIcon(VisualElement target, string iconName)
    {
        TrySetNamedIcon(target, iconName);
    }

    /// <summary>Same as <see cref="SetNamedIcon"/>; kept for call-site clarity on skill slots.</summary>
    public static void SetSkillIcon(VisualElement target, string iconName) =>
        SetNamedIcon(target, iconName);

    public static bool TrySetNamedIcon(VisualElement target, string iconName)
    {
        if (target == null || IconManager.Instance == null || string.IsNullOrEmpty(iconName))
            return false;

        Texture2D icon = IconManager.Instance.LoadTextureByName(iconName);
        if (icon == null)
            return false;

        target.style.backgroundImage = icon;
        return true;
    }

    /// <summary>Full Resources path (SkillLearn DF buttons, etc.).</summary>
    public static void SetResourcesBackground(VisualElement target, string resourcesPath)
    {
        if (target == null || string.IsNullOrEmpty(resourcesPath))
            return;

        Texture2D texture = IconManager.Instance != null
            ? IconManager.Instance.LoadTextureOtherSources(resourcesPath)
            : Resources.Load<Texture2D>(resourcesPath);

        if (texture != null)
            target.style.backgroundImage = new StyleBackground(texture);
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
