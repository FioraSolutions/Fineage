using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

public sealed partial class L2HtmlRenderer
{
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
}
