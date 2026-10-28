using System;
using UnityEngine;

/// <summary>
/// Local C# Fineage-Backend (login TCP 9106, zone TCP 9777) without changing the public default.
/// On for a player built with FINEAGE_LOCAL_BACKEND, or when -fineageLocal / FINEAGE_LOCAL_BACKEND=1 /
/// PlayerPrefs Fineage.LocalBackend=1 is set. -fineagePublic and FINEAGE_LOCAL_BACKEND=0 turn it off.
/// </summary>
public static class FineageLocalBackend
{
    public const string PrefKey = "Fineage.LocalBackend";
    public const int PlaytestScreenWidth = 1920;
    public const int PlaytestScreenHeight = 1080;

    public static readonly string PlaytestScreenArgs = "-screen-width 1920 -screen-height 1080 -screen-fullscreen 0";

    public static bool IsRequested()
    {
        if (HasArg("-fineagePublic"))
            return false;

        string env = Environment.GetEnvironmentVariable("FINEAGE_LOCAL_BACKEND");
        if (env == "0" || string.Equals(env, "false", StringComparison.OrdinalIgnoreCase))
            return false;

        if (HasArg("-fineageLocal"))
            return true;
        if (env == "1" || string.Equals(env, "true", StringComparison.OrdinalIgnoreCase))
            return true;
        if (PlayerPrefs.GetInt(PrefKey, 0) == 1)
            return true;

#if FINEAGE_LOCAL_BACKEND
        return true;
#else
        return false;
#endif
    }

    public static void ApplyIfRequested()
    {
        if (!IsRequested())
            return;

        string loginHost = FirstNonEmpty(ArgValue("-fineageLoginHost"), FineageWireConfig.LocalLoginHost);
        int loginPort = ParsePort(ArgValue("-fineageLoginPort"), FineageWireConfig.LocalLoginPort);
        string zoneHost = FirstNonEmpty(ArgValue("-fineageZoneHost"), FineageWireConfig.LocalZoneHost);
        int zonePort = ParsePort(ArgValue("-fineageZonePort"), FineageWireConfig.LocalZonePort);
        FineageWireConfig.EnableLocalBackend(loginHost, loginPort, zoneHost, zonePort);
        ApplyPlaytestWindow();
        Debug.Log("[FineageLocal] mode=local-backend login=" + loginHost + ":" + loginPort
            + " zone=" + zoneHost + ":" + zonePort);
    }

    /// <summary>Large windowed playtest on Mac Standalone (ProjectSettings are also set at local build time).</summary>
    static void ApplyPlaytestWindow()
    {
#if UNITY_STANDALONE
        if (HasArg("-screen-fullscreen") && ArgValue("-screen-fullscreen") == "1")
            return;

        int width = PlaytestScreenWidth;
        int height = PlaytestScreenHeight;
        string widthArg = ArgValue("-screen-width");
        string heightArg = ArgValue("-screen-height");
        if (!string.IsNullOrEmpty(widthArg) && int.TryParse(widthArg, out int parsedW) && parsedW > 0)
            width = parsedW;
        if (!string.IsNullOrEmpty(heightArg) && int.TryParse(heightArg, out int parsedH) && parsedH > 0)
            height = parsedH;

        if (Screen.fullScreen || Screen.width != width || Screen.height != height)
        {
            Screen.SetResolution(width, height, FullScreenMode.Windowed);
            Debug.Log("[FineageLocal] window " + width + "x" + height + " windowed");
        }
#endif
    }

    public static string CharacterHint
    {
        get
        {
            return FirstNonEmpty(
                ArgValue("-fineageChar"),
                Environment.GetEnvironmentVariable("FINEAGE_LOCAL_CHAR"),
                PlayerPrefs.GetString("Fineage.LocalCharacter", ""));
        }
    }

    public static bool QuitAfterSpawn
    {
        get { return HasArg("-fineageQuitAfterSpawn"); }
    }

    public static bool TryGetAutoLogin(out string account, out string password)
    {
        account = FirstNonEmpty(ArgValue("-fineageUser"), Environment.GetEnvironmentVariable("FINEAGE_LOCAL_USER"));
        password = FirstNonEmpty(ArgValue("-fineagePass"), Environment.GetEnvironmentVariable("FINEAGE_LOCAL_PASS"));
        if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(password))
        {
            account = null;
            password = null;
            return false;
        }

        return IsRequested();
    }

    static int ParsePort(string text, int fallback)
    {
        int port;
        if (int.TryParse(text, out port) && port >= 1 && port <= 65535)
            return port;
        return fallback;
    }

    static string FirstNonEmpty(params string[] values)
    {
        if (values == null)
            return "";
        for (int i = 0; i < values.Length; i++)
        {
            if (!string.IsNullOrWhiteSpace(values[i]))
                return values[i].Trim();
        }

        return "";
    }

    static bool HasArg(string name)
    {
        string[] args = Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == name)
                return true;
        }

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
