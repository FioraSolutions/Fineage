using System;
using UnityEngine;
using System.Globalization;

public static partial class L2StaticMeshSunMaskDatabase
{
    public static Vector3 ParseVector(string text)
    {
        if (string.IsNullOrEmpty(text))
            return Vector3.zero;
        return new Vector3(Field(text, "X"), Field(text, "Y"), Field(text, "Z"));
    }

    static float Field(string text, string name)
    {
        int start = text.IndexOf(name + "=", StringComparison.OrdinalIgnoreCase);
        if (start < 0)
            return 0f;
        start += name.Length + 1;
        int end = start;
        while (end < text.Length && text[end] != ',' && text[end] != ')')
            end++;
        float value;
        return float.TryParse(
            text.Substring(start, end - start),
            System.Globalization.NumberStyles.Float,
            System.Globalization.CultureInfo.InvariantCulture,
            out value)
            ? value
            : 0f;
    }
}
