using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

public sealed partial class L2HtmlRenderer
{
    public void Render(string html)
    {
        ClearState();
        _root.Clear();

        _root.style.overflow = Overflow.Hidden;
        _root.style.flexShrink = 1;
        _root.style.flexGrow = 1;
        _root.style.minWidth = 0;
        _root.style.maxWidth = Length.Percent(100);
        _root.style.width = Length.Percent(100);
        _root.style.alignItems = Align.Stretch;

        if (string.IsNullOrWhiteSpace(html))
            return;

        html = Decode(html);

        int i = 0;

        while (i < html.Length)
        {
            char c = html[i];

            if (c == '<')
            {
                int end = FindTagEnd(html, i + 1);

                if (end < 0)
                    break;

                string rawTag = html.Substring(i + 1, end - i - 1).Trim();

                string tagName = rawTag.StartsWith("/")
                    ? ReadTagName(rawTag.Substring(1).Trim()).ToLowerInvariant()
                    : ReadTagName(rawTag).ToLowerInvariant();

                bool inlineFormat = IsInlineFormatTag(tagName);

                if (!_insideHead && !_insideTitle)
                {
                    if (!inlineFormat)
                        FlushText();
                }
                else
                {
                    _textBuffer.Clear();
                }

                HandleTag(rawTag);

                i = end + 1;
            }
            else
            {
                if (!_insideHead && !_insideTitle)
                    _textBuffer.Append(c);

                i++;
            }
        }

        FlushText();
    }

    private void ClearState()
    {
        _parentStack.Clear();
        _colorStack.Clear();
        _fontSizeStack.Clear();
        _linkActionStack.Clear();
        _centerStack.Clear();
        _textAlignStack.Clear();
        _cellPaddingStack.Clear();
        _cellSpacingStack.Clear();
        _noWrapStack.Clear();
        _preStack.Clear();

        _boldStack.Clear();
        _italicStack.Clear();
        _underlineStack.Clear();

        _valueProviders.Clear();
        _radioGroups.Clear();

        _textBuffer.Clear();

        _activeColor = null;
        _activeFontSize = null;

        _boldMode = false;
        _italicMode = false;
        _underlineMode = false;
        _noWrapMode = false;
        _preMode = false;

        _centerMode = false;
        _insideHead = false;
        _insideTitle = false;

        _activeTextAlign = TextAnchor.MiddleLeft;

        _activeCellPadding = 1f;
        _activeCellSpacing = 0f;

        _activeDropdown = null;
        _activeDropdownVar = null;
        _activeDropdownSelectedIndex = -1;
        _activeDropdownLabels.Clear();
        _activeDropdownValues.Clear();
    }
}
