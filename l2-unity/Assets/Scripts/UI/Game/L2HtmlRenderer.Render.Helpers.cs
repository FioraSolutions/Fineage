using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

public sealed partial class L2HtmlRenderer
{
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
