using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

public sealed partial class L2HtmlRenderer
{
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
