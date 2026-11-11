using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

public sealed partial class L2HtmlRenderer
{
    private void HandleClosingTag(string closingName)
    {
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
    }
}
