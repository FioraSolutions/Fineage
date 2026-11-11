using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

public sealed partial class L2HtmlRenderer
{
    private void ApplyValign(VisualElement ve, string valign)
    {
        switch (valign.ToLowerInvariant())
        {
            case "top":
                ve.style.justifyContent = Justify.FlexStart;
                break;

            case "bottom":
                ve.style.justifyContent = Justify.FlexEnd;
                break;

            case "middle":
            case "center":
                ve.style.justifyContent = Justify.Center;
                break;

            default:
                ve.style.justifyContent = Justify.FlexStart;
                break;
        }
    }

    private void ApplyJustify(VisualElement ve, string justify)
    {
        switch (justify.ToLowerInvariant())
        {
            case "center":
                ve.style.justifyContent = Justify.Center;
                break;

            case "between":
                ve.style.justifyContent = Justify.SpaceBetween;
                break;

            case "around":
                ve.style.justifyContent = Justify.SpaceAround;
                break;

            case "end":
            case "right":
                ve.style.justifyContent = Justify.FlexEnd;
                break;

            default:
                ve.style.justifyContent = Justify.FlexStart;
                break;
        }
    }

    private void ApplySize(VisualElement ve)
    {
        string width = GetAttr("fixwidth", GetAttr("width", ""));

        if (TryFloat(width, out float w))
        {
            ve.style.width = w;
            ve.style.maxWidth = Length.Percent(100);
            ve.style.minWidth = 0;
            ve.style.flexShrink = 1;

            if (_attrs.ContainsKey("fixwidth"))
            {
                ve.style.flexGrow = 0;
                ve.style.flexShrink = 1;
                ve.style.minWidth = 0;
                ve.style.maxWidth = Length.Percent(100);
            }
        }

        if (TryFloat(GetAttr("height", ""), out float h))
            ve.style.height = h;

        if (TryFloat(GetAttr("minwidth", ""), out float minW))
            ve.style.minWidth = minW;

        if (TryFloat(GetAttr("maxwidth", ""), out float maxW))
            ve.style.maxWidth = maxW;

        if (TryFloat(GetAttr("minheight", ""), out float minH))
            ve.style.minHeight = minH;

        if (TryFloat(GetAttr("maxheight", ""), out float maxH))
            ve.style.maxHeight = maxH;
    }
}
