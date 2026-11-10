using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

public partial class L2HtmlRenderer
{
    private void AddMultiEdit()
    {
        TextField input = new TextField
        {
            multiline = true
        };

        input.AddToClassList("html_multiedit");

        input.pickingMode = PickingMode.Position;
        input.focusable = true;
        input.isDelayed = false;

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

        ResetBoxSpacing(input);
        PreventOverflow(input);
        ApplyTooltip(input);
        ApplyDisabled(input);

        CurrentParent.Add(input);
    }

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

    private void AddButton()
    {
        string value = GetAttr("value", "Button");
        string action = GetAttr("action", "");

        Button button = new Button();

        button.text = value;
        button.name = value;
        button.AddToClassList("html_button");

        button.pickingMode = PickingMode.Position;
        button.focusable = true;

        if (TryFloat(GetAttr("width", ""), out float width))
            button.style.width = width;

        if (TryFloat(GetAttr("height", ""), out float height))
            button.style.height = height;

        string fore = GetAttr("fore", "");
        string back = GetAttr("back", GetAttr("background", ""));

        Texture2D foreTexture = LoadTexture(fore);
        Texture2D backTexture = LoadTexture(back);

        if (foreTexture != null)
            button.style.backgroundImage = new StyleBackground(foreTexture);
        else if (backTexture != null)
            button.style.backgroundImage = new StyleBackground(backTexture);

        if (foreTexture != null || backTexture != null)
        {
            button.RegisterCallback<MouseEnterEvent>(_ =>
            {
                if (backTexture != null)
                    button.style.backgroundImage = new StyleBackground(backTexture);
            });

            button.RegisterCallback<MouseLeaveEvent>(_ =>
            {
                if (foreTexture != null)
                    button.style.backgroundImage = new StyleBackground(foreTexture);
                else if (backTexture != null)
                    button.style.backgroundImage = new StyleBackground(backTexture);
            });
        }

        ResetBoxSpacing(button);
        PreventOverflow(button);
        ApplyTooltip(button);
        ApplyDisabled(button);

        button.clicked += () =>
        {
            if (!button.enabledSelf)
                return;

            string finalAction = PrepareAction(action);

            if (!string.IsNullOrEmpty(finalAction))
                _onAction?.Invoke(finalAction);
        };

        CurrentParent.Add(button);
    }

    private void AddImage()
    {
        string src = GetAttr("src", "");

        if (string.IsNullOrEmpty(src))
            return;

        Image image = new Image();

        image.AddToClassList("html_image");

        Texture2D texture = LoadTexture(src);
        // Unity 6 UITK asserts in RepaintPanels / RenderChain when Image.image is null.
        if (texture == null)
            return;

        image.image = texture;

        if (TryFloat(GetAttr("width", ""), out float width))
            image.style.width = width;

        if (TryFloat(GetAttr("height", ""), out float height))
            image.style.height = height;

        ResetBoxSpacing(image);
        PreventOverflow(image);
        ApplyTooltip(image);

        string action = GetAttr("action", GetAttr("href", ""));

        if (!string.IsNullOrEmpty(action))
        {
            image.pickingMode = PickingMode.Position;
            image.focusable = true;

            image.RegisterCallback<ClickEvent>(evt =>
            {
                string finalAction = PrepareAction(action);

                if (!string.IsNullOrEmpty(finalAction))
                    _onAction?.Invoke(finalAction);

                evt.StopPropagation();
            });
        }

        CurrentParent.Add(image);
    }

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
