using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Globalization;
using System.Text;

public sealed partial class PlaytestEndpoint
{
    static bool ReadNumber(string text, ref int i, out string token)
    {
        int start = i;
        if (text[i] == '-')
            i++;
        int digits = i;
        while (i < text.Length && (char.IsDigit(text[i]) || text[i] == '.'))
            i++;
        token = text.Substring(start, i - start);
        return i > digits && token != "-";
    }

    static bool MatchLiteral(string text, ref int i, string literal)
    {
        if (i + literal.Length > text.Length)
            return false;
        if (string.Compare(text, i, literal, 0, literal.Length, StringComparison.Ordinal) != 0)
            return false;
        i += literal.Length;
        return true;
    }

    static bool ReadString(string text, ref int i, out string value)
    {
        value = null;
        if (i >= text.Length || text[i] != '"')
            return false;
        i++;
        var buffer = new System.Text.StringBuilder();
        while (i < text.Length)
        {
            char c = text[i++];
            if (c == '"')
            {
                value = buffer.ToString();
                return true;
            }
            if (c != '\\')
            {
                buffer.Append(c);
                continue;
            }
            if (i >= text.Length)
                return false;
            char esc = text[i++];
            switch (esc)
            {
                case '"':
                case '\\':
                case '/':
                    buffer.Append(esc);
                    break;
                case 'b':
                    buffer.Append('\b');
                    break;
                case 'f':
                    buffer.Append('\f');
                    break;
                case 'n':
                    buffer.Append('\n');
                    break;
                case 'r':
                    buffer.Append('\r');
                    break;
                case 't':
                    buffer.Append('\t');
                    break;
                case 'u':
                    if (i + 4 > text.Length)
                        return false;
                    int code;
                    if (!int.TryParse(text.Substring(i, 4), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out code))
                        return false;
                    buffer.Append((char)code);
                    i += 4;
                    break;
                default:
                    return false;
            }
        }
        return false;
    }
}
