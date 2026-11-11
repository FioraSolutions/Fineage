using UnityEngine;
using UnityEngine.UIElements;

public static partial class L2SlotIconView
{
    public static void SetItemIcon(VisualElement target, int itemId)
    {
        if (target == null || IconPacketServices.Icons == null)
            return;

        target.style.backgroundImage = new StyleBackground(IconPacketServices.Icons.GetIcon(itemId));
    }

    public static void SetItemIconAtIndex(VisualElement target, int itemId, int iconIndex)
    {
        if (target == null || IconPacketServices.Icons == null)
            return;

        target.style.backgroundImage = new StyleBackground(
            IconPacketServices.Icons.GetOtherIcon(itemId, iconIndex));
    }

    public static void SetNamedIcon(VisualElement target, string iconName)
    {
        TrySetNamedIcon(target, iconName);
    }

    public static void SetSkillIcon(VisualElement target, string iconName) =>
        SetNamedIcon(target, iconName);

    public static bool TrySetNamedIcon(VisualElement target, string iconName)
    {
        if (target == null || IconPacketServices.Icons == null || string.IsNullOrEmpty(iconName))
            return false;

        // Prefer the miss-aware loader so callers can skip Classic/unknown skill ids
        // instead of painting the NO IMAGE placeholder as a "successful" icon.
        Texture2D icon = IconPacketServices.Icons.TryLoadTextureByName(iconName);
        if (icon == null || IconPacketServices.Icons.IsNoImage(icon))
            return false;

        target.style.backgroundImage = icon;
        return true;
    }

    public static Texture2D LoadResourcesTexture(string resourcesPath)
    {
        if (string.IsNullOrEmpty(resourcesPath))
            return null;

        return IconPacketServices.Icons != null
            ? IconPacketServices.Icons.LoadTextureOtherSources(resourcesPath)
            : Resources.Load<Texture2D>(resourcesPath);
    }
}
