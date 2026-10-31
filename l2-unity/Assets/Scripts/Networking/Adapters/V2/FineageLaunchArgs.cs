using System;
using UnityEngine;

/// <summary>
/// Optional command-line helpers (headless smoke / auto-pick). They never change hosts:
/// the client always dials the public RP endpoints from <see cref="PlaytestEndpoint"/>.
/// </summary>
public static class FineageLaunchArgs
{
    public const string CharacterPrefKey = "Fineage.LocalCharacter";

    public static string CharacterHint
    {
        get { return FirstNonEmpty(ArgValue("-fineageChar"), PlayerPrefs.GetString(CharacterPrefKey, "")); }
    }

    /// <summary>Drop a leftover character name so the next login opens the lobby. Main thread only.</summary>
    public static void ClearCharacterHint()
    {
        if (!PlayerPrefs.HasKey(CharacterPrefKey))
            return;
        PlayerPrefs.DeleteKey(CharacterPrefKey);
        PlayerPrefs.Save();
    }

    public static bool QuitAfterSpawn
    {
        get { return HasArg("-fineageQuitAfterSpawn"); }
    }

    public static bool TryGetAutoLogin(out string account, out string password)
    {
        account = ArgValue("-fineageUser");
        password = ArgValue("-fineagePass");
        if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(password))
        {
            account = null;
            password = null;
            return false;
        }
        return true;
    }

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
