using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public sealed partial class EnterEventTable
{
    static string ClassName(string effect)
    {
        if (string.IsNullOrEmpty(effect))
            return string.Empty;
        effect = effect.Trim();
        int dot = effect.LastIndexOf('.');
        return dot >= 0 ? effect.Substring(dot + 1) : effect;
    }

    static string CleanSound(string raw)
    {
        if (string.IsNullOrEmpty(raw))
            return string.Empty;
        string sound = raw.Replace("\\0", string.Empty).Trim();
        if (sound.StartsWith("a,"))
            sound = sound.Substring(2);
        if (string.Equals(sound, "none", System.StringComparison.OrdinalIgnoreCase))
            return string.Empty;
        return sound;
    }
}
