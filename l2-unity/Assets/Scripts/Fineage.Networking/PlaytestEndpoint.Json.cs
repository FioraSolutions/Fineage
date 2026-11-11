using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

public sealed partial class PlaytestEndpoint
{
    static bool ReadValue(string text, ref int i, string key, ref PlaytestEndpointFields fields)
    {
        if (i >= text.Length)
            return false;
        if (text[i] == '"')
        {
            string value;
            if (!ReadString(text, ref i, out value))
                return false;
            AssignJsonString(ref fields, key, value);
            return true;
        }
        if (text[i] == '-' || char.IsDigit(text[i]))
        {
            string token;
            if (!ReadNumber(text, ref i, out token))
                return false;
            AssignJsonNumber(ref fields, key, token);
            return true;
        }
        if (MatchLiteral(text, ref i, "null"))
            return true;
        return false;
    }

    static void AssignJsonString(ref PlaytestEndpointFields fields, string key, string value)
    {
        if (key == "host")
        {
            fields.Host = value;
            fields.HostSet = true;
        }
        else if (key == "loginHost")
        {
            fields.LoginHost = value;
            fields.LoginHostSet = true;
        }
        else if (key == "gameHost")
        {
            fields.GameHost = value;
            fields.GameHostSet = true;
        }
        else if (key == "loginPort")
        {
            AssignPort(value, out fields.LoginPort, out fields.LoginPortSet);
        }
        else if (key == "gamePort")
        {
            AssignPort(value, out fields.GamePort, out fields.GamePortSet);
        }
    }

    static void AssignJsonNumber(ref PlaytestEndpointFields fields, string key, string token)
    {
        if (key == "loginPort")
            AssignPort(token, out fields.LoginPort, out fields.LoginPortSet);
        else if (key == "gamePort")
            AssignPort(token, out fields.GamePort, out fields.GamePortSet);
    }

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
