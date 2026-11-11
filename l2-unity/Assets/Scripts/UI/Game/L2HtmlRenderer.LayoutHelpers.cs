using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

public sealed partial class L2HtmlRenderer
{
    private VisualElement CreateContainer()
    {
        VisualElement ve = new VisualElement();

        ve.style.display = DisplayStyle.Flex;
        ve.style.visibility = Visibility.Visible;

        ve.style.flexGrow = 0;
        ve.style.flexShrink = 1;
        ve.style.minWidth = 0;
        ve.style.maxWidth = Length.Percent(100);

        ResetBoxSpacing(ve);

        return ve;
    }

    private void ResetBoxSpacing(VisualElement ve)
    {
        ve.style.marginTop = 0;
        ve.style.marginBottom = 0;
        ve.style.marginLeft = 0;
        ve.style.marginRight = 0;

        ve.style.paddingTop = 0;
        ve.style.paddingBottom = 0;
        ve.style.paddingLeft = 0;
        ve.style.paddingRight = 0;

        ve.style.minHeight = 0;
    }

    private void PreventOverflow(VisualElement ve)
    {
        ve.style.maxWidth = Length.Percent(100);
        ve.style.minWidth = 0;
        ve.style.flexShrink = 1;
    }

    private void PopParent()
    {
        if (_parentStack.Count > 0)
            _parentStack.Pop();
    }
}
