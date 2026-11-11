using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

public sealed partial class L2HtmlRenderer
{
    private void HandleTag(string rawTag)
    {
        if (string.IsNullOrWhiteSpace(rawTag))
            return;

        bool closing = rawTag[0] == '/';

        if (closing)
        {
            string closingName = rawTag.Substring(1).Trim().ToLowerInvariant();

            if (closingName == "head")
            {
                _insideHead = false;
                _textBuffer.Clear();
                return;
            }

            if (closingName == "title")
            {
                _insideTitle = false;
                _textBuffer.Clear();
                return;
            }

            if (_insideHead || _insideTitle)
            {
                _textBuffer.Clear();
                return;
            }

            switch (closingName)
            {
                case "div":
                case "p":
                case "tr":
                case "td":
                    PopParent();
                    PopTextAlign();
                    break;

                case "table":
                    PopParent();

                    if (_cellPaddingStack.Count > 0)
                        _activeCellPadding = _cellPaddingStack.Pop();

                    if (_cellSpacingStack.Count > 0)
                        _activeCellSpacing = _cellSpacingStack.Pop();

                    break;

                case "center":
                    PopParent();
                    _centerMode = _centerStack.Count > 0 && _centerStack.Pop();
                    PopTextAlign();
                    break;

                case "font":
                    if (_activeFontSize.HasValue)
                        _textBuffer.Append("</size>");

                    if (!string.IsNullOrEmpty(_activeColor))
                        _textBuffer.Append("</color>");

                    _activeColor = _colorStack.Count > 0 ? _colorStack.Pop() : null;
                    _activeFontSize = _fontSizeStack.Count > 0 ? _fontSizeStack.Pop() : null;
                    break;

                case "a":
                    if (_linkActionStack.Count > 0)
                        _linkActionStack.Pop();
                    break;

                case "b":
                    _textBuffer.Append("</b>");
                    _boldMode = _boldStack.Count > 0 && _boldStack.Pop();
                    break;

                case "i":
                    _textBuffer.Append("</i>");
                    _italicMode = _italicStack.Count > 0 && _italicStack.Pop();
                    break;

                case "u":
                    _textBuffer.Append("</u>");
                    _underlineMode = _underlineStack.Count > 0 && _underlineStack.Pop();
                    break;

                case "nobr":
                    _noWrapMode = _noWrapStack.Count > 0 && _noWrapStack.Pop();
                    break;

                case "pre":
                    _preMode = _preStack.Count > 0 && _preStack.Pop();
                    break;

                case "select":
                case "combobox":
                    FinishDropdown();
                    break;

                case "option":
                    FinishDropdownOptionText();
                    break;
            }

            return;
        }

        string tagName = ReadTagName(rawTag).ToLowerInvariant();

        if (tagName == "head")
        {
            _insideHead = true;
            _textBuffer.Clear();
            return;
        }

        if (tagName == "title")
        {
            _insideTitle = true;
            _textBuffer.Clear();
            return;
        }

        ReadAttributes(rawTag, _attrs);

        if (tagName == "body")
        {
            ApplyBackground(_root);
            ApplyTooltip(_root);
            return;
        }

        if (_insideHead || _insideTitle)
        {
            _textBuffer.Clear();
            return;
        }

        if (tagName == "html")
            return;

        switch (tagName)
        {
            case "br":
            case "br1":
                AddBreak();
                break;

            case "hr":
                AddHr();
                break;

            case "center":
                AddCenter();
                break;

            case "p":
                AddParagraph();
                break;

            case "pre":
                _preStack.Push(_preMode);
                _preMode = true;
                break;

            case "nobr":
                _noWrapStack.Push(_noWrapMode);
                _noWrapMode = true;
                break;

            case "font":
                {
                    _colorStack.Push(_activeColor);
                    _fontSizeStack.Push(_activeFontSize);

                    string newColor = NormalizeColor(GetAttr("color", GetAttr("value", _activeColor ?? "#DCD9DC")));
                    _activeColor = newColor;

                    _textBuffer.Append("<color=");
                    _textBuffer.Append(newColor);
                    _textBuffer.Append(">");

                    if (TryFloat(GetAttr("size", ""), out float size))
                    {
                        _activeFontSize = ConvertFontSize(size);

                        _textBuffer.Append("<size=");
                        _textBuffer.Append(Mathf.RoundToInt(_activeFontSize.Value));
                        _textBuffer.Append(">");
                    }

                    break;
                }

            case "b":
                _boldStack.Push(_boldMode);
                _boldMode = true;
                _textBuffer.Append("<b>");
                break;

            case "i":
                _italicStack.Push(_italicMode);
                _italicMode = true;
                _textBuffer.Append("<i>");
                break;

            case "u":
                _underlineStack.Push(_underlineMode);
                _underlineMode = true;
                _textBuffer.Append("<u>");
                break;

            case "a":
                _linkActionStack.Push(GetAttr("action", GetAttr("href", "")));
                break;

            case "div":
                AddDiv();
                break;

            case "table":
                AddTable();
                break;

            case "tr":
                AddRow();
                break;

            case "td":
                AddCell();
                break;

            case "multiedit":
                AddMultiEdit();
                break;

            case "edit":
            case "input":
                AddInput();
                break;

            case "button":
                AddButton();
                break;

            case "img":
                AddImage();
                break;

            case "select":
            case "combobox":
                StartDropdown();
                break;

            case "option":
                AddDropdownOption();
                break;

            case "checkbox":
                AddCheckbox();
                break;

            case "radio":
                AddRadio();
                break;
        }
    }

    private bool IsInlineFormatTag(string tagName)
    {
        return tagName == "font" ||
               tagName == "b" ||
               tagName == "i" ||
               tagName == "u" ||
               tagName == "nobr";
    }

    private int FindTagEnd(string html, int start)
    {
        bool insideQuote = false;
        char quoteChar = '\0';

        for (int i = start; i < html.Length; i++)
        {
            char c = html[i];

            if (insideQuote)
            {
                if (c == quoteChar)
                {
                    insideQuote = false;
                    quoteChar = '\0';
                }

                continue;
            }

            if (c == '"' || c == '\'')
            {
                insideQuote = true;
                quoteChar = c;
                continue;
            }

            if (c == '>')
                return i;
        }

        return -1;
    }
}
