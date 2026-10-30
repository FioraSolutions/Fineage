using System;
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
    static Mode? _prefsCache;

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
        EnablePlaytestBackend(
            string.IsNullOrWhiteSpace(loginHost) ? LocalLoginHost : loginHost,
            loginPort > 0 ? loginPort : LocalLoginPort,
            string.IsNullOrWhiteSpace(zoneHost) ? LocalZoneHost : zoneHost,
            zonePort > 0 ? zonePort : LocalZonePort);
    }

    /// <summary>
    /// ZoneV2 login handoff for Fineage-Backend. Empty hosts use the public playtest endpoints.
    /// Interlude Blowfish login hangs on this TCP service.
    /// </summary>
    public static void EnablePlaytestBackend(
        string loginHost,
        int loginPort,
        string zoneHost,
        int zonePort)
    {
        SetOverride(Mode.ZoneV2);
        LoginHost = string.IsNullOrWhiteSpace(loginHost) ? PlaytestEndpoint.DefaultLoginHost : loginHost.Trim();
        LoginPort = loginPort > 0 ? loginPort : PlaytestEndpoint.DefaultLoginPort;
        ZoneHost = string.IsNullOrWhiteSpace(zoneHost) ? PlaytestEndpoint.DefaultGameHost : zoneHost.Trim();
        ZonePort = zonePort > 0 ? zonePort : PlaytestEndpoint.DefaultGamePort;
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
            if (_prefsCache.HasValue)
                return _prefsCache.Value;
            try
            {
                _prefsCache = (Mode)PlayerPrefs.GetInt(PrefKey, (int)Mode.Interlude);
            }
            catch (Exception)
            {
                return Mode.Interlude;
            }

            return _prefsCache.Value;
        }
        set
        {
            _prefsCache = value;
            try
            {
                PlayerPrefs.SetInt(PrefKey, (int)value);
                PlayerPrefs.Save();
            }
            catch (Exception)
            {
                /* ignore off-thread */
            }
        }
    }

    public static bool IsZoneV2
    {
        get { return Current == Mode.ZoneV2; }
    }

    /// <summary>Public playtest login TCP (e.g. login.fineage.evpoint.by:2106). Not loopback or localhost.</summary>
    public static bool IsPublicLoginHost(string host)
    {
        if (string.IsNullOrWhiteSpace(host))
            return false;
        string trimmed = host.Trim();
        if (trimmed == "127.0.0.1" || trimmed == "::1" || trimmed == "0.0.0.0")
            return false;
        if (trimmed.StartsWith("127.", StringComparison.Ordinal))
            return false;
        return !string.Equals(trimmed, "localhost", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Log In must call zone <c>BeginSession</c> when ZoneV2, login handoff, or the
    /// transport bypasses Interlude — even if <c>LoginClient</c> is already connected.
    /// </summary>
    public static bool LoginPressStartsZoneSession(bool transportBypassesInterludeLogin)
    {
        return UseLoginHandoff || IsZoneV2 || transportBypassesInterludeLogin;
    }
}
