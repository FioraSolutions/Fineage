using System;
using System.Globalization;

public static partial class EnchantGlow
{
    public static bool TryParseClassic(string value, out EnchantGlowPlacement right, out EnchantGlowPlacement left)
    {
        right = EnchantGlowPlacement.None;
        left = EnchantGlowPlacement.None;
        if (string.IsNullOrWhiteSpace(value))
            return false;

        string text = value.Trim();
        int eq = text.IndexOf('=');
        if (eq >= 0 && eq < 12)
            text = text.Substring(eq + 1).Trim();

        int found = 0;
        int cursor = 0;
        while (cursor < text.Length)
        {
            int open = text.IndexOf('[', cursor);
            if (open < 0)
                break;
            int close = text.IndexOf(']', open + 1);
            if (close < 0)
                break;
            string mesh = text.Substring(open + 1, close - open - 1).Trim();
            cursor = close + 1;

            if (!TryReadVec3(text, ref cursor, out float sx, out float sy, out float sz))
                break;
            if (!TryReadVec3(text, ref cursor, out float ox, out float oy, out float oz))
            {
                ox = 0f;
                oy = 0f;
                oz = 0f;
            }

            var placement = EnchantGlowPlacement.From(mesh, sx, sy, sz, ox, oy, oz);
            if (found == 0)
                right = placement;
            else if (found == 1)
                left = placement;
            found++;
            if (found >= 2)
                break;
        }

        return found > 0;
    }

    public static EnchantGlowPlacement ParseInterlude(string mesh, string sx, string sy, string sz, string ox, string oy, string oz)
    {
        return EnchantGlowPlacement.From(
            mesh,
            ParseFloat(sx, 1f),
            ParseFloat(sy, 1f),
            ParseFloat(sz, 1f),
            ParseFloat(ox, 0f),
            ParseFloat(oy, 0f),
            ParseFloat(oz, 0f));
    }

    public static bool TryParseInterludeRow(string[] cols, out EnchantGlowPlacement right, out EnchantGlowPlacement left)
    {
        right = EnchantGlowPlacement.None;
        left = EnchantGlowPlacement.None;
        if (cols == null || cols.Length <= InterludeOffsetBX + 2)
            return false;
        right = ParseInterlude(
            cols[InterludeRangeA],
            Col(cols, InterludeScaleAX),
            Col(cols, InterludeScaleAX + 1),
            Col(cols, InterludeScaleAX + 2),
            Col(cols, InterludeOffsetAX),
            Col(cols, InterludeOffsetAX + 1),
            Col(cols, InterludeOffsetAX + 2));
        left = ParseInterlude(
            cols[InterludeRangeB],
            Col(cols, InterludeScaleBX),
            Col(cols, InterludeScaleBX + 1),
            Col(cols, InterludeScaleBX + 2),
            Col(cols, InterludeOffsetBX),
            Col(cols, InterludeOffsetBX + 1),
            Col(cols, InterludeOffsetBX + 2));
        return right.Present || left.Present;
    }

    static string Col(string[] cols, int index)
    {
        return index >= 0 && index < cols.Length ? cols[index] : "";
    }

    static bool TryReadVec3(string text, ref int cursor, out float x, out float y, out float z)
    {
        x = y = z = 0f;
        int open = text.IndexOf('{', cursor);
        if (open < 0)
            return false;
        int close = text.IndexOf('}', open + 1);
        if (close < 0)
            return false;
        string inner = text.Substring(open + 1, close - open - 1);
        cursor = close + 1;
        string[] parts = inner.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 3)
            return false;
        x = ParseFloat(parts[0], 0f);
        y = ParseFloat(parts[1], 0f);
        z = ParseFloat(parts[2], 0f);
        return true;
    }

    static float ParseFloat(string text, float fallback)
    {
        if (string.IsNullOrWhiteSpace(text))
            return fallback;
        if (float.TryParse(text.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out float value))
            return value;
        return fallback;
    }
}
