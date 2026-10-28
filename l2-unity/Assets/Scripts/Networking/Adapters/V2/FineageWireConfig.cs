using UnityEngine;

/// <summary>
/// Feature flag for Interlude stub vs Fineage ZoneServer v2.
/// Public playtest hosts come from <see cref="PlaytestEndpoint"/> defaults.
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

    public static void EnableLocalBackend(
        string loginHost,
        int loginPort,
        string zoneHost,
        int zonePort)
    {
        SetOverride(Mode.ZoneV2);
        LoginHost = string.IsNullOrWhiteSpace(loginHost) ? LocalLoginHost : loginHost;
        LoginPort = loginPort > 0 ? loginPort : LocalLoginPort;
        ZoneHost = string.IsNullOrWhiteSpace(zoneHost) ? LocalZoneHost : zoneHost;
        ZonePort = zonePort > 0 ? zonePort : LocalZonePort;
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
