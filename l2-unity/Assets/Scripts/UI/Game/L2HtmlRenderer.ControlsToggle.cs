using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

public sealed partial class L2HtmlRenderer
{
    private void AddCheckbox()
    {
        Toggle toggle = new Toggle();

        toggle.AddToClassList("html_checkbox");

        string label = GetAttr("label", GetAttr("value", ""));
        string name = GetAttr("var", GetAttr("name", ""));

        toggle.text = label;

        if (IsTruthy(GetAttr("checked", "")))
            toggle.value = true;

        if (!string.IsNullOrEmpty(name))
        {
            string checkedValue = GetAttr("value", "1");
            string uncheckedValue = GetAttr("unchecked", "0");

            _valueProviders[name] = () => toggle.value ? checkedValue : uncheckedValue;
        }

        if (TryFloat(GetAttr("width", ""), out float width))
            toggle.style.width = width;

        if (TryFloat(GetAttr("height", ""), out float height))
            toggle.style.height = height;

        ResetBoxSpacing(toggle);
        PreventOverflow(toggle);
        ApplyTooltip(toggle);
        ApplyDisabled(toggle);

        CurrentParent.Add(toggle);
    }

    private void AddRadio()
    {
        RadioButton radio = new RadioButton();

        radio.AddToClassList("html_radio");

        string group = GetAttr("var", GetAttr("name", ""));
        string value = GetAttr("value", "");
        string label = GetAttr("label", value);

        radio.text = label;

        if (IsTruthy(GetAttr("checked", "")))
            radio.value = true;

        if (!string.IsNullOrEmpty(group))
        {
            radio.RegisterValueChangedCallback(evt =>
            {
                if (!evt.newValue)
                    return;

                foreach (KeyValuePair<string, RadioButton> pair in _radioGroups)
                {
                    if (pair.Key.StartsWith(group + ":", StringComparison.Ordinal) && pair.Value != radio)
                        pair.Value.value = false;
                }
            });

            _radioGroups[group + ":" + value] = radio;

            _valueProviders[group] = () =>
            {
                foreach (KeyValuePair<string, RadioButton> pair in _radioGroups)
                {
                    if (pair.Key.StartsWith(group + ":", StringComparison.Ordinal) && pair.Value.value)
                        return pair.Key.Substring(group.Length + 1);
                }

                return string.Empty;
            };
        }

        ResetBoxSpacing(radio);
        PreventOverflow(radio);
        ApplyTooltip(radio);
        ApplyDisabled(radio);

        CurrentParent.Add(radio);
    }
}
