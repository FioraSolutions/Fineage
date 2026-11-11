using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

public sealed partial class L2HtmlRenderer
{
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
}
