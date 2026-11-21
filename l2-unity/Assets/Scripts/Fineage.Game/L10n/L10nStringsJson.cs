using System;
using System.Collections.Generic;
using System.Text;

/// <summary>Parses flat <c>{ "key": "value", ... }</c> locale tables (no arrays, one object depth).</summary>
public static class L10nStringsJson
{
    public static Dictionary<string, string> Parse(string json)
    {
        var table = new Dictionary<string, string>(StringComparer.Ordinal);
        if (string.IsNullOrWhiteSpace(json))
            return table;

        int i = 0;
        SkipWs(json, ref i);
        if (i >= json.Length || json[i] != '{')
            throw new InvalidOperationException("strings.json must be a JSON object.");

        i++;
        while (i < json.Length)
        {
            SkipWs(json, ref i);
            if (i < json.Length && json[i] == '}')
            {
                i++;
                break;
            }

            string key = ReadString(json, ref i);
            SkipWs(json, ref i);
            if (i >= json.Length || json[i] != ':')
                throw new InvalidOperationException("strings.json: expected ':' after key.");
            i++;
            SkipWs(json, ref i);
            string value = ReadString(json, ref i);
            table[key] = value;
            SkipWs(json, ref i);
            if (i < json.Length && json[i] == ',')
                i++;
        }

        return table;
    }

    static void SkipWs(string json, ref int i)
    {
        while (i < json.Length && char.IsWhiteSpace(json[i]))
            i++;
    }

    static string ReadString(string json, ref int i)
    {
        if (i >= json.Length || json[i] != '"')
            throw new InvalidOperationException("strings.json: expected string.");
        i++;
        var sb = new StringBuilder();
        while (i < json.Length)
        {
            char c = json[i++];
            if (c == '"')
                return sb.ToString();
            if (c == '\\')
            {
                if (i >= json.Length)
                    throw new InvalidOperationException("strings.json: truncated escape.");
                char esc = json[i++];
                switch (esc)
                {
                    case '"': sb.Append('"'); break;
                    case '\\': sb.Append('\\'); break;
                    case '/': sb.Append('/'); break;
                    case 'b': sb.Append('\b'); break;
                    case 'f': sb.Append('\f'); break;
                    case 'n': sb.Append('\n'); break;
                    case 'r': sb.Append('\r'); break;
                    case 't': sb.Append('\t'); break;
                    case 'u':
                        if (i + 4 > json.Length)
                            throw new InvalidOperationException("strings.json: bad unicode escape.");
                        string hex = json.Substring(i, 4);
                        i += 4;
                        sb.Append((char)Convert.ToInt32(hex, 16));
                        break;
                    default:
                        sb.Append(esc);
                        break;
                }
            }
            else
            {
                sb.Append(c);
            }
        }

        throw new InvalidOperationException("strings.json: unterminated string.");
    }
}