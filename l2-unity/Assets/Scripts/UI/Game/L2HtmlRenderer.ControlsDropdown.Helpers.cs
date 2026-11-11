using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

public sealed partial class L2HtmlRenderer
{
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
