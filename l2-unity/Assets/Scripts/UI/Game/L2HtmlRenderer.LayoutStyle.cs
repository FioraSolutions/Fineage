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

    private void ApplyClasses(VisualElement ve)
    {
        string className = GetAttr("class", "");

        if (string.IsNullOrEmpty(className))
            return;

        string[] classes = className.Split(' ');

        foreach (string cls in classes)
        {
            if (!string.IsNullOrWhiteSpace(cls))
                ve.AddToClassList(cls);
        }
    }

    private void ApplyTooltip(VisualElement ve)
    {
        string tooltip = GetAttr("tooltip", GetAttr("title", ""));

        if (!string.IsNullOrEmpty(tooltip))
            ve.tooltip = tooltip;
    }

    private void ApplyDisabled(VisualElement ve)
    {
        string disabled = GetAttr("disabled", "");

        if (IsTruthy(disabled))
            ve.SetEnabled(false);
    }
}
