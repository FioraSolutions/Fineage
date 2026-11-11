using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

public abstract class AbstractToggle
{
    private readonly string[] fillBackgroundDf =
    {
        L2ExpandCollapseUiAssets.ExpandButton,
        L2ExpandCollapseUiAssets.CollapseButton,
    };

    protected void ClickDf(Button btn, int[] arrDfSelect , VisualElement dataElement , int indexButton)
    {
        bool show = arrDfSelect[indexButton] == 0;
        ChangeDfBox(btn, fillBackgroundDf[show ? 0 : 1]);
        arrDfSelect[indexButton] = show ? 1 : 0;
        ToogleHideElement(dataElement, show);
    }

    protected void ToogleHideElement(VisualElement element, bool isHide)
    {
        if (!isHide)
            element.style.display = DisplayStyle.Flex;
        else
            element.style.display = DisplayStyle.None;
    }

    protected void ChangeDfBox(Button btn, string texture)
    {
        IEnumerable<VisualElement> children = btn.Children();
        var e = children.First();
        e.style.display = DisplayStyle.Flex;
        L2SlotIconView.SetResourcesBackground(btn, texture);
    }
}
