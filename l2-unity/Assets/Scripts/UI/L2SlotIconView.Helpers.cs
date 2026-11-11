using UnityEngine;
using UnityEngine.UIElements;

public static partial class L2SlotIconView
{
    public static void SetResourcesBackground(VisualElement target, string resourcesPath) =>
        TrySetResourcesBackground(target, resourcesPath);

    public static bool TrySetResourcesBackground(
        VisualElement target,
        string resourcesPath,
        bool useNativeSize = false)
    {
        if (target == null)
            return false;

        Texture2D texture = LoadResourcesTexture(resourcesPath);
        if (texture == null)
            return false;

        target.style.backgroundImage = new StyleBackground(texture);
        if (useNativeSize)
        {
            target.style.width = texture.width;
            target.style.height = texture.height;
        }

        return true;
    }

    public static void SetEmptyInventoryBackground(VisualElement target)
    {
        if (target == null || IconPacketServices.Icons == null)
            return;

        target.style.backgroundImage = new StyleBackground(
            IconPacketServices.Icons.GetInvetoryDefaultBackground());
    }

    public static void PaintEmptyInventorySlot(VisualElement slotBg, VisualElement slotElement)
    {
        if (slotElement == null)
            return;

        if (slotBg != null)
            SetEmptyInventoryBackground(slotBg);
        else
            Clear(slotElement);
    }

    public static void Clear(VisualElement target)
    {
        if (target == null)
            return;

        target.style.backgroundImage = null;
    }

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
