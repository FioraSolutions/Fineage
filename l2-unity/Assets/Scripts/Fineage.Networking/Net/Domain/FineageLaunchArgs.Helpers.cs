using System;
using UnityEngine;

public static partial class FineageLaunchArgs
{
    static string FirstNonEmpty(params string[] values)
    {
        for (int i = 0; i < values.Length; i++)
            if (!string.IsNullOrWhiteSpace(values[i]))
                return values[i].Trim();
        return "";
    }

    static bool HasArg(string name)
    {
        foreach (string a in Environment.GetCommandLineArgs())
            if (a == name)
                return true;
        return false;
    }

    static string ArgValue(string name)
    {
        string[] args = Environment.GetCommandLineArgs();
        string prefix = name + "=";
        for (int i = 0; i < args.Length; i++)
        {
            string arg = args[i];
            if (arg == name && i + 1 < args.Length)
                return args[i + 1];
            if (arg != null && arg.StartsWith(prefix, StringComparison.Ordinal))
                return arg.Substring(prefix.Length);
        }
        return null;
    }
}
