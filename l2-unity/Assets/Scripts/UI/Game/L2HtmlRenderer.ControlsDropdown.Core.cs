using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

public sealed partial class L2HtmlRenderer
{
    private void StartDropdown()
    {
        _activeDropdown = new DropdownField();
        _activeDropdown.AddToClassList("html_dropdown");

        _activeDropdownVar = GetAttr("var", GetAttr("name", ""));
        _activeDropdownSelectedIndex = -1;

        _activeDropdownLabels.Clear();
        _activeDropdownValues.Clear();
        _textBuffer.Clear();

        if (TryFloat(GetAttr("width", ""), out float width))
            _activeDropdown.style.width = width;

        if (TryFloat(GetAttr("height", ""), out float height))
            _activeDropdown.style.height = height;

        string selected = GetAttr("selected", "");

        if (TryInt(selected, out int selectedIndex))
            _activeDropdownSelectedIndex = selectedIndex;

        ResetBoxSpacing(_activeDropdown);
        PreventOverflow(_activeDropdown);
        ApplyTooltip(_activeDropdown);
        ApplyDisabled(_activeDropdown);
    }

    private void AddDropdownOption()
    {
        if (_activeDropdown == null)
            return;

        string value = GetAttr("value", "");
        string label = GetAttr("label", "");

        _activeDropdownLabels.Add(label);
        _activeDropdownValues.Add(value);

        if (IsTruthy(GetAttr("selected", "")))
            _activeDropdownSelectedIndex = _activeDropdownLabels.Count - 1;

        _textBuffer.Clear();
    }
}
