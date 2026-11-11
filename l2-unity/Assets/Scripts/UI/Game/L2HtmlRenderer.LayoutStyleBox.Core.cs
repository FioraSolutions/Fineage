using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

public sealed partial class L2HtmlRenderer
{
    private void ApplyTableAlign(VisualElement ve)
    {
        string align = GetAttr("align", "").ToLowerInvariant();

        if (align == "center")
            ve.style.alignSelf = Align.Center;
        else if (align == "right" || align == "end")
            ve.style.alignSelf = Align.FlexEnd;
        else if (align == "left" || align == "start")
            ve.style.alignSelf = Align.FlexStart;
    }

    private void ApplyAlign(VisualElement ve, string align)
    {
        switch (align.ToLowerInvariant())
        {
            case "center":
                ve.style.alignItems = Align.Center;
                break;

            case "right":
            case "end":
                ve.style.alignItems = Align.FlexEnd;
                break;

            case "stretch":
                ve.style.alignItems = Align.Stretch;
                break;

            default:
                ve.style.alignItems = Align.FlexStart;
                break;
        }
    }
}
