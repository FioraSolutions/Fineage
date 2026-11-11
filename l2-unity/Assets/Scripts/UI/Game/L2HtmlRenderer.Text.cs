using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

public sealed partial class L2HtmlRenderer
{
    private void FlushText()
    {
        if (_textBuffer.Length == 0)
            return;

        if (_activeDropdown != null)
            return;

        string text = _preMode
            ? _textBuffer.ToString()
            : NormalizeL2Text(_textBuffer.ToString());

        _textBuffer.Clear();

        if (string.IsNullOrWhiteSpace(text))
            return;

        bool isLink = !string.IsNullOrEmpty(CurrentLinkAction);
        bool hasRichColor = text.Contains("<color=");

        Label label = new Label();

        label.text = isLink ? "<u>" + text + "</u>" : text;
        label.enableRichText = true;

        label.AddToClassList(isLink ? "html_link" : "html_text");

        label.style.color = isLink && !hasRichColor ? LinkColor : Color.white;
        label.style.fontSize = 13;
        label.style.flexShrink = 1;
        label.style.minWidth = 0;
        label.style.maxWidth = Length.Percent(100);

        label.style.letterSpacing = 0;

        label.style.whiteSpace = (_preMode || _noWrapMode)
            ? WhiteSpace.NoWrap
            : WhiteSpace.Normal;

        label.style.unityTextAlign = _activeTextAlign;

        label.style.marginTop = 0;
        label.style.marginBottom = 0;
        label.style.paddingTop = 1;
        label.style.paddingBottom = 1;

        if (isLink)
        {
            string action = CurrentLinkAction;

            label.pickingMode = PickingMode.Position;
            label.focusable = true;

            label.RegisterCallback<MouseEnterEvent>(_ =>
            {
                if (!hasRichColor)
                    label.style.color = LinkHoverColor;
            });

            label.RegisterCallback<MouseLeaveEvent>(_ =>
            {
                if (!hasRichColor)
                    label.style.color = LinkColor;
            });

            label.RegisterCallback<ClickEvent>(evt =>
            {
                string finalAction = PrepareAction(action);

                if (!string.IsNullOrEmpty(finalAction))
                    _onAction?.Invoke(finalAction);

                evt.StopPropagation();
            });
        }

        CurrentParent.Add(label);
    }

    private void AddBreak()
    {
        VisualElement br = new VisualElement();

        br.AddToClassList("html_br");
        br.style.height = 6;
        br.style.flexShrink = 0;
        ResetBoxSpacing(br);

        CurrentParent.Add(br);
    }

    private void PushTextAlign(TextAnchor align)
    {
        _textAlignStack.Push(_activeTextAlign);
        _activeTextAlign = align;
    }

    private void PopTextAlign()
    {
        _activeTextAlign = _textAlignStack.Count > 0
            ? _textAlignStack.Pop()
            : TextAnchor.MiddleLeft;
    }

    private string NormalizeL2Text(string text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        text = text.Replace("\r", " ")
                   .Replace("\n", " ")
                   .Replace("\t", " ")
                   .Replace("&nbsp;", " ");

        // collapse ONLY extreme spacing, keep single spaces
        return Regex.Replace(text, @" {2,}", " ");
    }

    private float ConvertFontSize(float htmlSize)
    {
        if (htmlSize <= 1) return 10;
        if (htmlSize == 2) return 12;
        if (htmlSize == 3) return 13;
        if (htmlSize == 4) return 15;
        if (htmlSize == 5) return 17;
        if (htmlSize == 6) return 19;

        return htmlSize;
    }

    private TextAnchor ParseTextAlign(string align)
    {
        switch (align.ToLowerInvariant())
        {
            case "center":
                return TextAnchor.MiddleCenter;

            case "right":
            case "end":
                return TextAnchor.MiddleRight;

            default:
                return TextAnchor.MiddleLeft;
        }
    }
}
