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
}
