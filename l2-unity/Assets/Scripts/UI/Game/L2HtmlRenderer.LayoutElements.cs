public sealed partial class L2HtmlRenderer
{
    private void AddHr()
    {
        VisualElement hr = new VisualElement();

        hr.AddToClassList("html_hr");

        hr.style.height = TryFloat(GetAttr("height", ""), out float h) ? h : 1;
        hr.style.width = Length.Percent(100);
        hr.style.maxWidth = Length.Percent(100);
        hr.style.flexShrink = 1;

        Color color = new Color32(90, 90, 90, 255);

        string colorAttr = GetAttr("color", GetAttr("bordercolor", ""));

        if (!string.IsNullOrEmpty(colorAttr))
        {
            string normalized = NormalizeColor(colorAttr);

            if (ColorUtility.TryParseHtmlString(normalized, out Color parsed))
                color = parsed;
        }

        hr.style.backgroundColor = color;
        hr.style.marginTop = 3;
        hr.style.marginBottom = 3;
        hr.style.marginLeft = 0;
        hr.style.marginRight = 0;

        CurrentParent.Add(hr);
    }

    private void AddCenter()
    {
        _centerStack.Push(_centerMode);
        _centerMode = true;

        PushTextAlign(TextAnchor.MiddleCenter);

        VisualElement center = CreateContainer();

        center.AddToClassList("html_center");
        center.style.flexDirection = FlexDirection.Column;
        center.style.alignItems = Align.Center;
        center.style.justifyContent = Justify.FlexStart;
        center.style.width = Length.Percent(100);
        center.style.maxWidth = Length.Percent(100);
        center.style.minWidth = 0;
        center.style.flexShrink = 1;

        CurrentParent.Add(center);
        _parentStack.Push(center);
    }

    private void AddParagraph()
    {
        VisualElement p = CreateContainer();

        p.AddToClassList("html_p");

        p.style.flexDirection = FlexDirection.Column;
        p.style.width = Length.Percent(100);
        p.style.maxWidth = Length.Percent(100);
        p.style.minWidth = 0;
        p.style.flexShrink = 1;

        string align = GetAttr("align", _centerMode ? "center" : "left");

        ApplyAlign(p, align);
        PushTextAlign(ParseTextAlign(align));

        ApplyJustify(p, GetAttr("justify", "start"));
        ApplySize(p);
        PreventOverflow(p);
        ApplyBackground(p);
        ApplyBorder(p);
        ApplyBoxSpacing(p);
        ApplyClasses(p);
        ApplyTooltip(p);

        CurrentParent.Add(p);
        _parentStack.Push(p);
    }

    private void AddDiv()
    {
        VisualElement div = CreateContainer();

        div.AddToClassList("html_div");

        string flex = GetAttr("flex", "column");
        div.style.flexDirection = flex == "row" ? FlexDirection.Row : FlexDirection.Column;

        string align = GetAttr("align", _centerMode ? "center" : "start");

        ApplyAlign(div, align);
        PushTextAlign(ParseTextAlign(align));

        ApplyJustify(div, GetAttr("justify", "start"));
        ApplySize(div);
        PreventOverflow(div);
        ApplyBackground(div);
        ApplyBorder(div);
        ApplyBoxSpacing(div);
        ApplyClasses(div);
        ApplyTooltip(div);

        CurrentParent.Add(div);
        _parentStack.Push(div);
    }

    private void AddTable()
    {
        _cellPaddingStack.Push(_activeCellPadding);
        _cellSpacingStack.Push(_activeCellSpacing);

        if (TryFloat(GetAttr("cellpadding", ""), out float padding))
            _activeCellPadding = padding;

        if (TryFloat(GetAttr("cellspacing", ""), out float spacing))
            _activeCellSpacing = spacing;

        VisualElement table = CreateContainer();

        table.AddToClassList("html_table");

        table.style.overflow = Overflow.Visible;
        table.style.flexDirection = FlexDirection.Column;
        table.style.alignItems = Align.Stretch;
        table.style.justifyContent = Justify.FlexStart;

        table.style.maxWidth = Length.Percent(100);
        table.style.minWidth = 0;
        table.style.flexShrink = 1;

        if (!_attrs.ContainsKey("width") && !_attrs.ContainsKey("fixwidth"))
            table.style.width = Length.Percent(100);

        ApplySize(table);
        PreventOverflow(table);
        ApplyTableAlign(table);
        ApplyBackground(table);
        ApplyBorder(table);
        ApplyBoxSpacing(table);
        ApplyClasses(table);
        ApplyTooltip(table);

        CurrentParent.Add(table);
        _parentStack.Push(table);
    }

    private void AddRow()
    {
        VisualElement row = CreateContainer();

        row.AddToClassList("html_tr");

        row.style.flexDirection = FlexDirection.Row;
        row.style.alignItems = Align.Center;
        row.style.justifyContent = Justify.FlexStart;
        row.style.width = Length.Percent(100);
        row.style.maxWidth = Length.Percent(100);
        row.style.minWidth = 0;
        row.style.flexShrink = 1;
        row.style.flexWrap = Wrap.NoWrap;

        ApplySize(row);
        PreventOverflow(row);
        ApplyBackground(row);
        ApplyBorder(row);
        ApplyBoxSpacing(row);
        ApplyClasses(row);
        ApplyTooltip(row);

        PushTextAlign(_activeTextAlign);

        CurrentParent.Add(row);
        _parentStack.Push(row);
    }

    private void AddCell()
    {
        VisualElement cell = CreateContainer();

        cell.AddToClassList("html_td");

        cell.style.flexDirection = FlexDirection.Column;

        bool fixedWidth = _attrs.ContainsKey("fixwidth") || _attrs.ContainsKey("width");

        if (fixedWidth)
        {
            cell.style.flexGrow = 0;
            cell.style.flexShrink = 1;
            cell.style.minWidth = 0;
            cell.style.maxWidth = Length.Percent(100);
        }
        else
        {
            cell.style.flexGrow = 1;
            cell.style.flexShrink = 1;
            cell.style.minWidth = 0;
        }

        cell.style.minWidth = 0;
        cell.style.maxWidth = Length.Percent(100);
        cell.style.flexDirection = FlexDirection.Row;
        cell.style.flexWrap = Wrap.NoWrap;

        float padding = _activeCellPadding;

        if (TryFloat(GetAttr("padding", ""), out float customPadding))
            padding = customPadding;

        cell.style.overflow = Overflow.Visible;
        cell.style.paddingLeft = padding;
        cell.style.paddingRight = padding;
        cell.style.paddingTop = padding;
        cell.style.paddingBottom = padding;

        if (_activeCellSpacing > 0)
        {
            cell.style.marginLeft = _activeCellSpacing;
            cell.style.marginRight = _activeCellSpacing;
            cell.style.marginTop = _activeCellSpacing;
            cell.style.marginBottom = _activeCellSpacing;
        }

        string align = GetAttr("align", _centerMode ? "center" : "start");

        ApplyAlign(cell, align);
        PushTextAlign(ParseTextAlign(align));
        ApplyValign(cell, GetAttr("valign", "middle"));

        bool previousNoWrap = _noWrapMode;

        if (_attrs.ContainsKey("nowrap") || IsTruthy(GetAttr("nowrap", "")))
            _noWrapMode = true;

        ApplySize(cell);

        if (TryInt(GetAttr("colspan", ""), out int colspan) && colspan > 1)
            cell.style.flexGrow = colspan;

        PreventOverflow(cell);
        ApplyBackground(cell);
        ApplyBorder(cell);
        ApplyBoxSpacing(cell, false);
        ApplyClasses(cell);
        ApplyTooltip(cell);

        cell.RegisterCallback<DetachFromPanelEvent>(_ =>
        {
            _noWrapMode = previousNoWrap;
        });

        CurrentParent.Add(cell);
        _parentStack.Push(cell);
    }

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
