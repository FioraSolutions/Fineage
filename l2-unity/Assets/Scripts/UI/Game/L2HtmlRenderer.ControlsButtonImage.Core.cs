using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

public sealed partial class L2HtmlRenderer
{
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
}
