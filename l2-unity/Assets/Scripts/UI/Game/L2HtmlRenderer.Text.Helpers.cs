using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

public sealed partial class L2HtmlRenderer
{
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
