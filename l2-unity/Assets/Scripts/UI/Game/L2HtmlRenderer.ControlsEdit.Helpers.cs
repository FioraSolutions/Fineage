using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

public sealed partial class L2HtmlRenderer
{
    private void AddInput()
    {
        TextField input = new TextField();

        input.AddToClassList("html_input");

        string name = GetAttr("var", GetAttr("name", ""));

        if (!string.IsNullOrEmpty(name))
            _valueProviders[name] = () => input.value ?? string.Empty;

        if (TryFloat(GetAttr("width", ""), out float width))
            input.style.width = width;

        if (TryFloat(GetAttr("height", ""), out float height))
            input.style.height = height;

        if (TryInt(GetAttr("maxlength", ""), out int maxLength))
            input.maxLength = maxLength;

        input.value = GetAttr("value", "");

        if (IsTruthy(GetAttr("readonly", "")))
            input.isReadOnly = true;

        if (GetAttr("type", "").ToLowerInvariant() == "password")
            input.isPasswordField = true;

        ResetBoxSpacing(input);
        PreventOverflow(input);
        ApplyTooltip(input);
        ApplyDisabled(input);

        CurrentParent.Add(input);
    }
}
