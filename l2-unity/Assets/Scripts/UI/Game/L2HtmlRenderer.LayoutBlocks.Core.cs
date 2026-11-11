using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

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
}
