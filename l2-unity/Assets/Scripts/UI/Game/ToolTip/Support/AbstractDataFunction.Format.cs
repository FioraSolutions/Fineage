using UnityEngine;
using UnityEngine.UIElements;

public abstract partial class AbstractDataFunction
{
    protected void AddElementEnchantifNot0(VisualElement groupPriceLabel, Label priceLabel, int enchant)
    {
        if (groupPriceLabel != null)
        {
            if (enchant == 0)
            {
                AddElementIfNotEmpty(groupPriceLabel, priceLabel, null);
            }
            else
            {
                AddElementIfNotEmpty(groupPriceLabel, priceLabel, "+" + enchant);
            }
        }

    }

    protected void AddElementIfNotEmpty(VisualElement groupElement, Label labelData, string text)
    {
        if (groupElement != null)
        {
            if (!string.IsNullOrEmpty(text))
            {
                labelData.text = text;
                groupElement.style.display = DisplayStyle.Flex;
            }
            else
            {
                groupElement.style.display = DisplayStyle.None;
                labelData.text = "";
            }
        }

    }

    protected void AddElementIfNot0(VisualElement groupElement, Label labelData, int addParam)
    {
        if (addParam != 0 && addParam != -1)
        {
            labelData.text = addParam.ToString();
            groupElement.style.display = DisplayStyle.Flex;
        }
        else
        {
            if (groupElement == null)
            {
                // Debug.LogWarning(" ToolTipDataProvider: Не критическая ошибка мы не нашли элемент tooltips ");
                return;
            }
            groupElement.style.display = DisplayStyle.None;
            labelData.text = "";
        }
    }

    protected void AddElementIfNot0(VisualElement groupElement, Label labelData, double addParam)
    {
        if (addParam != 0 && addParam != -1)
        {
            labelData.text = ToolTipsUtils.ConvertNumberToNormal(addParam.ToString());
            groupElement.style.display = DisplayStyle.Flex;
        }
        else
        {
            if (groupElement == null)
            {
                // Debug.LogWarning(" ToolTipDataProvider: Не критическая ошибка мы не нашли элемент tooltips ");
                return;
            }
            groupElement.style.display = DisplayStyle.None;
            labelData.text = "";
        }
    }

    public void AddElementIfNotNullVisibleHide(VisualElement groupElement, Label labelData, int addParam)
    {
        if (addParam != 0 && addParam != -1)
        {
            labelData.text = addParam.ToString();
            groupElement.style.visibility = Visibility.Visible;
        }
        else
        {
            if (groupElement == null)
            {
                // Debug.LogWarning(" ToolTipDataProvider: Не критическая ошибка мы не нашли элемент tooltips ");
                return;
            }
            groupElement.style.visibility = Visibility.Hidden;
            labelData.text = "";
        }
    }

    protected void SetImageElement(VisualElement element, Texture2D texture)
    {
        if (element != null)
        {
            if (texture == null)
            {
                element.style.display = DisplayStyle.None;
            }
            else
            {
                element.style.display = DisplayStyle.Flex;
                element.style.backgroundImage = texture;
            }
        }

    }

    protected void EnabledRow(VisualElement element)
    {
        if (element != null)
        {
            element.style.display = DisplayStyle.Flex;
        }
    }
}
