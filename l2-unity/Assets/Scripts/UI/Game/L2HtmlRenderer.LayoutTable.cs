using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

public sealed partial class L2HtmlRenderer
{
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
}
