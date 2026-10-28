using UnityEngine;

/// <summary>
/// Feature flag for Interlude stub vs Fineage ZoneServer v2.
/// Public playtest calls <see cref="EnablePlaytestBackend"/> (ZoneV2 + login handoff).
/// <see cref="FineageLocalBackend"/> opt-in calls <see cref="EnableLocalBackend"/> for 127.0.0.1.
/// </summary>
public static class FineageWireConfig
{
    public const string PrefKey = "Fineage.WireMode";

    public enum Mode
    {
        Interlude = 0,
        ZoneV2 = 1
    }

    static Mode? _override;

    /// <summary>Local C# Fineage-Backend login (only after <see cref="EnableLocalBackend"/>).</summary>
    public const string LocalLoginHost = "127.0.0.1";
    public const int LocalLoginPort = 9106;
    public const string LocalZoneHost = "127.0.0.1";
    public const int LocalZonePort = 9777;

    public static string ZoneHost = PlaytestEndpoint.DefaultGameHost;
    public static int ZonePort = PlaytestEndpoint.DefaultGamePort;

    /// <summary>Fineage-Backend login TCP when <see cref="UseLoginHandoff"/> is set.</summary>
    public static string LoginHost = PlaytestEndpoint.DefaultLoginHost;
    public static int LoginPort = PlaytestEndpoint.DefaultLoginPort;

    /// <summary>
    /// When true, <see cref="ZoneV2Client.BeginSession"/> logs in on <see cref="LoginHost"/>
    /// and sends the handoff token as the zone Auth password.
    /// </summary>
    public static bool UseLoginHandoff;

    /// <summary>
    /// Public Fineage login + zone. ZoneV2 and login handoff, with playtest hosts.
    /// Empty values fall back to <see cref="PlaytestEndpoint"/> defaults, not loopback.
    /// </summary>
    public static void EnablePlaytestBackend(
        string loginHost,
        int loginPort,
        string zoneHost,
        int zonePort)
    {
        ApplyZoneV2Handoff(
            loginHost,
            loginPort,
            zoneHost,
            zonePort,
            PlaytestEndpoint.DefaultLoginHost,
            PlaytestEndpoint.DefaultLoginPort,
            PlaytestEndpoint.DefaultGameHost,
            PlaytestEndpoint.DefaultGamePort);
    }

    public static void EnableLocalBackend(
        string loginHost,
        int loginPort,
        string zoneHost,
        int zonePort)
    {
        ApplyZoneV2Handoff(
            loginHost,
            loginPort,
            zoneHost,
            zonePort,
            LocalLoginHost,
            LocalLoginPort,
            LocalZoneHost,
            LocalZonePort);
    }

    static void ApplyZoneV2Handoff(
        string loginHost,
        int loginPort,
        string zoneHost,
        int zonePort,
        string fallbackLoginHost,
        int fallbackLoginPort,
        string fallbackZoneHost,
        int fallbackZonePort)
    {
        SetOverride(Mode.ZoneV2);
        LoginHost = string.IsNullOrWhiteSpace(loginHost) ? fallbackLoginHost : loginHost.Trim();
        LoginPort = loginPort > 0 ? loginPort : fallbackLoginPort;
        ZoneHost = string.IsNullOrWhiteSpace(zoneHost) ? fallbackZoneHost : zoneHost.Trim();
        ZonePort = zonePort > 0 ? zonePort : fallbackZonePort;
        UseLoginHandoff = true;
    }

    public static void SetOverride(Mode mode)
    {
        _override = mode;
    }

    public static void ClearOverride()
    {
        _override = null;
    }

    public static Mode Current
    {
        get
        {
            if (_override.HasValue)
                return _override.Value;
            return (Mode)PlayerPrefs.GetInt(PrefKey, (int)Mode.Interlude);
        }
        set
        {
            PlayerPrefs.SetInt(PrefKey, (int)value);
            PlayerPrefs.Save();
        }
    }

    public static bool IsZoneV2
    {
        get { return Current == Mode.ZoneV2; }
    }
}
