using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
        bool expand = arrDfSelect[indexButton] == 0;
        SetSectionExpanded(btn, arrDfSelect, indexButton, dataElement, expand);
    }

  /// <summary>
  /// Interlude MagicSkillWnd DF button: arr 1 = expanded (rows visible), 0 = collapsed.
  /// </summary>
    protected void SetSectionExpanded(Button btn, int[] arrDfSelect, int indexButton, VisualElement body, bool expanded)
    {
        if (arrDfSelect != null && indexButton >= 0 && indexButton < arrDfSelect.Length)
            arrDfSelect[indexButton] = expanded ? 1 : 0;
        ChangeDfBox(btn, fillBackgroundDf[expanded ? 1 : 0]);
        ToogleHideElement(body, !expanded);
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
        Texture2D image = L2UiResources.LoadTexture(texture);
        if (image != null)
            btn.style.backgroundImage = new StyleBackground(image);
    }
}
