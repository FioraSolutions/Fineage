using System;
using System.Collections.Generic;
using System.Linq;

public partial class DatUtils
{
    public static string CleanupString(string name) {
        return name.Replace("[", string.Empty).Replace("]", string.Empty);
    }

    public static string CleanupStringNew(string name)
    {
        return name.Replace("{", string.Empty).Replace("}", string.Empty);
    }

    public static string CleanupStringNewN(string name)
    {
        return name.Replace("\\n", string.Empty).Replace("\\", string.Empty);
    }

    public static string CleanupStringOldData(string name)
    {
        if (string.IsNullOrEmpty(name))
            return name;
        // Interlude meta uses literal "\n" escapes inside a,...\0 fields.
        return name
            .Replace("a,", string.Empty)
            .Replace("\\0", string.Empty)
            .Replace("\\n", "\n")
            .Replace("\\t", "\t");
    }

    public static string[] ParseArray(string value) {
        return SplitJSON(value);
    }
}
