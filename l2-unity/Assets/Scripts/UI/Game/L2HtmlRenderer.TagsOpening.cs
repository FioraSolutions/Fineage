using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

public sealed partial class L2HtmlRenderer
{
    private void HandleOpeningTag(string tagName)
    {
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
}
