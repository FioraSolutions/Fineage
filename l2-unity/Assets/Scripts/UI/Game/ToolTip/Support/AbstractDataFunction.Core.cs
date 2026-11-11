using UnityEngine;
using UnityEngine.UIElements;

public abstract partial class AbstractDataFunction
{
    protected void AddBoxIfNotEmpty(VisualElement boxElement, string text)
    {
        if (boxElement != null)
        {
            if (string.IsNullOrEmpty(text))
            {

                boxElement.style.display = DisplayStyle.None;
            }
            else
            {
                boxElement.style.display = DisplayStyle.Flex;
            }
        }

    }

    protected void AddElementIfNotNull(VisualElement groupElement, VisualElement icon, Texture2D texture)
    {
        if (texture != null)
        {
            icon.style.backgroundImage = texture;
            groupElement.style.display = DisplayStyle.Flex;
        }
        else
        {
            groupElement.style.display = DisplayStyle.None;
            L2SlotIconView.Clear(icon);
        }
    }

    protected void AddNamedIconIfPresent(VisualElement groupElement, VisualElement icon, string iconName)
    {
        if (L2SlotIconView.TrySetNamedIcon(icon, iconName))
        {
            groupElement.style.display = DisplayStyle.Flex;
            return;
        }

        groupElement.style.display = DisplayStyle.None;
        L2SlotIconView.Clear(icon);
    }

    protected void AddItemIconIfPresent(VisualElement groupElement, VisualElement icon, int itemId)
    {
        if (groupElement == null || icon == null || IconPacketServices.Icons == null)
        {
            if (groupElement != null)
                groupElement.style.display = DisplayStyle.None;
            return;
        }

        L2SlotIconView.SetItemIcon(icon, itemId);
        groupElement.style.display = DisplayStyle.Flex;
    }

    protected void AddResourcesIconIfPresent(
        VisualElement groupElement,
        VisualElement icon,
        string resourcesPath)
    {
        if (groupElement == null || icon == null)
            return;

        if (L2SlotIconView.TrySetResourcesBackground(icon, resourcesPath))
        {
            groupElement.style.display = DisplayStyle.Flex;
            return;
        }

        groupElement.style.display = DisplayStyle.None;
        L2SlotIconView.Clear(icon);
    }

    protected void AddElementPriceifNot0(VisualElement groupPriceLabel, Label priceLabel, int price)
    {
        if (price == 0 | price == -1)
        {
            AddElementIfNotEmpty(groupPriceLabel, priceLabel, null);
        }
        else
        {
            AddElementIfNotEmpty(groupPriceLabel, priceLabel, ToolTipsUtils.ConvertToPrice(price) + " Adena");
        }
    }

    protected void AddElementCountXifNot0(VisualElement groupPriceLabel, Label priceLabel, int price)
    {
        if (price == 0)
        {
            AddElementIfNotEmpty(groupPriceLabel, priceLabel, null);
        }
        else
        {
            AddElementIfNotEmpty(groupPriceLabel, priceLabel, ToolTipsUtils.ConvertToPrice(price));
        }
    }
}
