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

    private void FinishDropdownOptionText()
    {
        if (_activeDropdown == null || _activeDropdownLabels.Count == 0)
            return;

        int index = _activeDropdownLabels.Count - 1;

        string label = _activeDropdownLabels[index];
        string value = _activeDropdownValues[index];
        string innerText = NormalizeL2Text(_textBuffer.ToString()).Trim();

        _textBuffer.Clear();

        if (string.IsNullOrEmpty(label))
            label = !string.IsNullOrEmpty(innerText) ? innerText : value;

        if (string.IsNullOrEmpty(value))
            value = label;

        _activeDropdownLabels[index] = label;
        _activeDropdownValues[index] = value;
    }

    private void FinishDropdown()
    {
        if (_activeDropdown == null)
            return;

        FinishDropdownOptionText();

        _activeDropdown.choices = new List<string>(_activeDropdownLabels);

        if (_activeDropdownLabels.Count > 0)
        {
            int index = Mathf.Clamp(_activeDropdownSelectedIndex, 0, _activeDropdownLabels.Count - 1);
            _activeDropdown.value = _activeDropdownLabels[index];
        }

        if (!string.IsNullOrEmpty(_activeDropdownVar))
        {
            string varName = _activeDropdownVar;
            DropdownField dropdown = _activeDropdown;
            List<string> labels = new List<string>(_activeDropdownLabels);
            List<string> values = new List<string>(_activeDropdownValues);

            _valueProviders[varName] = () =>
            {
                int index = labels.IndexOf(dropdown.value);

                if (index >= 0 && index < values.Count)
                    return values[index];

                return dropdown.value ?? string.Empty;
            };
        }

        CurrentParent.Add(_activeDropdown);

        _activeDropdown = null;
        _activeDropdownVar = null;
        _activeDropdownSelectedIndex = -1;
        _activeDropdownLabels.Clear();
        _activeDropdownValues.Clear();
        _textBuffer.Clear();
    }
}
