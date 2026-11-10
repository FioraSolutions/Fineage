using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

public partial class L2HtmlRenderer
{
    private string PrepareAction(string action)
    {
        if (string.IsNullOrEmpty(action))
            return string.Empty;

        action = action.Replace("bypass", "").Replace("-h", "").Trim();

        return ReplaceVariables(action);
    }

    private string ReplaceVariables(string action)
    {
        action = VariableRegex.Replace(action, match =>
        {
            string key = match.Groups[1].Value;

            if (_valueProviders.TryGetValue(key, out Func<string> provider))
                return provider.Invoke();

            return match.Value;
        });

        action = PercentVariableRegex.Replace(action, match =>
        {
            string key = match.Groups[1].Value;

            if (_valueProviders.TryGetValue(key, out Func<string> provider))
                return provider.Invoke();

            return match.Value;
        });

        return action;
    }

    private string NormalizeColor(string color)
    {
        if (string.IsNullOrWhiteSpace(color))
            return "#DCD9DC";

        color = color.Trim();

        switch (color.ToLowerInvariant())
        {
            case "level":
                return "#FFD700";
            case "name":
                return "#FFFFFF";
            case "title":
                return "#B0C4DE";
            case "red":
                return "#FF0000";
            case "green":
                return "#00FF00";
            case "blue":
                return "#0000FF";
            case "white":
                return "#FFFFFF";
            case "black":
                return "#000000";
            case "yellow":
                return "#FFFF00";
            case "orange":
                return "#FFA500";
            case "gray":
            case "grey":
                return "#808080";
        }

        if (color.StartsWith("#"))
            return color;

        if (HexColorRegex.IsMatch(color))
            return "#" + color;

        if (ColorUtility.TryParseHtmlString(color, out Color parsed))
            return "#" + ColorUtility.ToHtmlStringRGB(parsed);

        return "#DCD9DC";
    }

    private string ReadTagName(string tagContent)
    {
        int index = tagContent.IndexOfAny(new[]
        {
            ' ',
            '\t',
            '\r',
            '\n',
            '/'
        });

        if (index == -1)
            return tagContent.Trim();

        return tagContent.Substring(0, index).Trim();
    }

    private void ReadAttributes(string tagContent, Dictionary<string, string> target)
    {
        target.Clear();

        int i = 0;
        int length = tagContent.Length;

        while (i < length && !char.IsWhiteSpace(tagContent[i]) && tagContent[i] != '/')
            i++;

        while (i < length)
        {
            while (i < length && (char.IsWhiteSpace(tagContent[i]) || tagContent[i] == '/'))
                i++;

            if (i >= length)
                break;

            int keyStart = i;

            while (i < length &&
                   !char.IsWhiteSpace(tagContent[i]) &&
                   tagContent[i] != '=' &&
                   tagContent[i] != '/')
            {
                i++;
            }

            if (i <= keyStart)
                break;

            string key = tagContent.Substring(keyStart, i - keyStart).ToLowerInvariant();

            while (i < length && char.IsWhiteSpace(tagContent[i]))
                i++;

            if (i >= length || tagContent[i] != '=')
            {
                target[key] = string.Empty;
                continue;
            }

            i++;

            while (i < length && char.IsWhiteSpace(tagContent[i]))
                i++;

            if (i >= length)
            {
                target[key] = string.Empty;
                break;
            }

            string value;
            char quote = tagContent[i];

            if (quote == '"' || quote == '\'')
            {
                i++;
                int valueStart = i;

                while (i < length && tagContent[i] != quote)
                    i++;

                value = tagContent.Substring(valueStart, i - valueStart);

                if (i < length && tagContent[i] == quote)
                    i++;
            }
            else
            {
                int valueStart = i;

                while (i < length && !char.IsWhiteSpace(tagContent[i]) && tagContent[i] != '/')
                    i++;

                value = tagContent.Substring(valueStart, i - valueStart);
            }

            target[key] = Decode(value);
        }
    }

    private string GetAttr(string key, string defaultValue)
    {
        return _attrs.TryGetValue(key.ToLowerInvariant(), out string value) ? value : defaultValue;
    }

    private bool TryFloat(string value, out float result)
    {
        return float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out result);
    }

    private bool TryInt(string value, out int result)
    {
        return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
    }

    private bool IsTruthy(string value)
    {
        if (string.IsNullOrEmpty(value))
            return false;

        value = value.Trim().ToLowerInvariant();

        return value == "true" ||
               value == "1" ||
               value == "yes" ||
               value == "checked" ||
               value == "selected" ||
               value == "disabled" ||
               value == "readonly" ||
               value == "nowrap";
    }

    private string Decode(string text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        return text
            .Replace("&nbsp;", " ")
            .Replace("&lt;", "<")
            .Replace("&gt;", ">")
            .Replace("&quot;", "\"")
            .Replace("&amp;", "&");
    }

    private Texture2D LoadTexture(string path)
    {
        if (string.IsNullOrEmpty(path))
            return null;

        if (TextureCache.TryGetValue(path, out Texture2D cached))
            return cached;

        if (MissingTextureCache.Contains(path))
            return null;

        Texture2D texture = Resources.Load<Texture2D>(path);

        if (texture != null)
        {
            TextureCache[path] = texture;
            return texture;
        }

        MissingTextureCache.Add(path);
        // Debug.LogWarning("L2HtmlRenderer: texture not found in Resources: " + path);
        return null;
    }
}
