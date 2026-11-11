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

        HandleClosingTag(closingName);

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

        HandleOpeningTag(tagName);
    }
}
