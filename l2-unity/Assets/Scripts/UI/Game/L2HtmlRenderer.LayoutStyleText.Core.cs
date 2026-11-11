using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

public sealed partial class L2HtmlRenderer
{
    private void ApplyBackground(VisualElement ve)
    {
        string bgColor = GetAttr("bgcolor", GetAttr("background-color", ""));

        if (!string.IsNullOrEmpty(bgColor))
        {
            string normalized = NormalizeColor(bgColor);

            if (ColorUtility.TryParseHtmlString(normalized, out Color parsed))
                ve.style.backgroundColor = parsed;
        }

        string background = GetAttr("background", GetAttr("bg", ""));

        if (string.IsNullOrEmpty(background))
            return;

        Texture2D texture = LoadTexture(background);

        if (texture != null)
            ve.style.backgroundImage = new StyleBackground(texture);
    }

    private void ApplyBorder(VisualElement ve)
    {
        string border = GetAttr("border", "");

        if (!TryFloat(border, out float width) || width <= 0)
            return;

        Color color = new Color32(90, 90, 90, 255);

        string borderColor = GetAttr("bordercolor", GetAttr("border-color", ""));

        if (!string.IsNullOrEmpty(borderColor))
        {
            string normalized = NormalizeColor(borderColor);

            if (ColorUtility.TryParseHtmlString(normalized, out Color parsed))
                color = parsed;
        }

        ve.style.borderTopWidth = width;
        ve.style.borderBottomWidth = width;
        ve.style.borderLeftWidth = width;
        ve.style.borderRightWidth = width;

        ve.style.borderTopColor = color;
        ve.style.borderBottomColor = color;
        ve.style.borderLeftColor = color;
        ve.style.borderRightColor = color;
    }

    private void ApplyBoxSpacing(VisualElement ve, bool allowPaddingOverride = true)
    {
        if (TryFloat(GetAttr("margin", ""), out float margin))
        {
            ve.style.marginTop = margin;
            ve.style.marginBottom = margin;
            ve.style.marginLeft = margin;
            ve.style.marginRight = margin;
        }

        if (TryFloat(GetAttr("margintop", ""), out float mt))
            ve.style.marginTop = mt;

        if (TryFloat(GetAttr("marginbottom", ""), out float mb))
            ve.style.marginBottom = mb;

        if (TryFloat(GetAttr("marginleft", ""), out float ml))
            ve.style.marginLeft = ml;

        if (TryFloat(GetAttr("marginright", ""), out float mr))
            ve.style.marginRight = mr;

        if (allowPaddingOverride && TryFloat(GetAttr("padding", ""), out float padding))
        {
            ve.style.paddingTop = padding;
            ve.style.paddingBottom = padding;
            ve.style.paddingLeft = padding;
            ve.style.paddingRight = padding;
        }

        if (allowPaddingOverride && TryFloat(GetAttr("paddingtop", ""), out float pt))
            ve.style.paddingTop = pt;

        if (allowPaddingOverride && TryFloat(GetAttr("paddingbottom", ""), out float pb))
            ve.style.paddingBottom = pb;

        if (allowPaddingOverride && TryFloat(GetAttr("paddingleft", ""), out float pl))
            ve.style.paddingLeft = pl;

        if (allowPaddingOverride && TryFloat(GetAttr("paddingright", ""), out float pr))
            ve.style.paddingRight = pr;
    }
}
