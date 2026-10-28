using UnityEngine;

/// <summary>
/// Feature flag for Interlude stub vs Fineage ZoneServer v2.
/// The GameManager inspector toggle sets the mode. <see cref="FineageLocalBackend"/>
/// runs after that and, when requested, points login and zone at the local C# backend.
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

    public const string DefaultLoginHost = "127.0.0.1";
    public const int DefaultLoginPort = 9106;
    public const string DefaultZoneHost = "127.0.0.1";
    public const int DefaultZonePort = 9777;

    public static string ZoneHost = DefaultZoneHost;
    public static int ZonePort = DefaultZonePort;

    /// <summary>Fineage-Backend login TCP. Used only when <see cref="UseLoginHandoff"/> is set.</summary>
    public static string LoginHost = DefaultLoginHost;
    public static int LoginPort = DefaultLoginPort;

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
        LoginHost = string.IsNullOrWhiteSpace(loginHost) ? DefaultLoginHost : loginHost;
        LoginPort = loginPort > 0 ? loginPort : DefaultLoginPort;
        ZoneHost = string.IsNullOrWhiteSpace(zoneHost) ? DefaultZoneHost : zoneHost;
        ZonePort = zonePort > 0 ? zonePort : DefaultZonePort;
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
