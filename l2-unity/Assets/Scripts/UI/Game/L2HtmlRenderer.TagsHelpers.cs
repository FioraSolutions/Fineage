using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

public sealed partial class L2HtmlRenderer
{

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
