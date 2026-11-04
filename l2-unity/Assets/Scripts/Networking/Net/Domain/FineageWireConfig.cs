using System;
using UnityEngine;

/// <summary>
/// Client wire selection: Interlude stub vs Fineage Net (<c>PacketEnvelope</c>).
/// The MessagePack V2 mode is gone; <see cref="Mode.Net"/> is the only Fineage path.
/// Public playtest hosts come from <see cref="PlaytestEndpoint"/> defaults.
/// </summary>
public static class FineageWireConfig
{
    public const string PrefKey = "Fineage.WireMode";

    public enum Mode
    {
        /// <summary>Interlude login stub (Blowfish) plus the Interlude game socket.</summary>
        Interlude = 0,

        /// <summary>Fineage Net on 9106/9777 locally, or the tunnel hosts on a playtest build.</summary>
        Net = 2
    }

    static Mode? _override;
    static Mode? _prefsCache;

    public static string ZoneHost = PlaytestEndpoint.DefaultGameHost;
    public static int ZonePort = PlaytestEndpoint.DefaultGamePort;

    /// <summary>Fineage-Backend login TCP when <see cref="UseLoginHandoff"/> is set.</summary>
    public static string LoginHost = PlaytestEndpoint.DefaultLoginHost;
    public static int LoginPort = PlaytestEndpoint.DefaultLoginPort;

    /// <summary>
    /// When true, <see cref="FineageZoneTransport.BeginSession"/> logs in on <see cref="LoginHost"/>
    /// and sends the handoff token as the zone Auth password.
    /// </summary>
    public static bool UseLoginHandoff;

    /// <summary>
    /// Fineage Net against the given hosts (local profile or public playtest tunnel).
    /// Empty hosts fall back to the public playtest endpoints. Blowfish login hangs on
    /// this TCP service, so the handoff token is always used.
    /// </summary>
    public static void EnablePlaytestBackend(
        string loginHost,
        int loginPort,
        string zoneHost,
        int zonePort)
    {
        SetOverride(Mode.Net);
        LoginHost = string.IsNullOrWhiteSpace(loginHost) ? PlaytestEndpoint.DefaultLoginHost : loginHost.Trim();
        LoginPort = loginPort > 0 ? loginPort : PlaytestEndpoint.DefaultLoginPort;
        ZoneHost = string.IsNullOrWhiteSpace(zoneHost) ? PlaytestEndpoint.DefaultGameHost : zoneHost.Trim();
        ZonePort = zonePort > 0 ? zonePort : PlaytestEndpoint.DefaultGamePort;
        UseLoginHandoff = true;
    }

    /// <summary>
    /// Local Fineage Net path (PacketEnvelope). Requires backend <c>FINEAGE_NET_ENABLE=1</c>
    /// on the forwarded ports 9106/9777.
    /// </summary>
    public static void EnableLocalNetBackend(
        string loginHost = "127.0.0.1",
        int loginPort = 9106,
        string zoneHost = "127.0.0.1",
        int zonePort = 9777)
    {
        SetOverride(Mode.Net);
        LoginHost = string.IsNullOrWhiteSpace(loginHost) ? "127.0.0.1" : loginHost.Trim();
        LoginPort = loginPort > 0 ? loginPort : 9106;
        ZoneHost = string.IsNullOrWhiteSpace(zoneHost) ? "127.0.0.1" : zoneHost.Trim();
        ZonePort = zonePort > 0 ? zonePort : 9777;
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
                _prefsCache = Normalize(PlayerPrefs.GetInt(PrefKey, (int)Mode.Interlude));
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

    /// <summary>
    /// Prefs written by older builds may still hold the retired <c>ZoneV2 = 1</c> value.
    /// Map anything unknown onto <see cref="Mode.Interlude"/> instead of failing to parse.
    /// </summary>
    static Mode Normalize(int stored)
    {
        return stored == (int)Mode.Net ? Mode.Net : Mode.Interlude;
    }

    public static bool IsNet
    {
        get { return Current == Mode.Net; }
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
    /// Log In must call zone <c>BeginSession</c> when a Net handoff is configured, or when the
    /// transport bypasses the Interlude login — even if <c>LoginClient</c> is already connected.
    /// </summary>
    public static bool LoginPressStartsZoneSession(bool transportBypassesInterludeLogin)
    {
        return UseLoginHandoff || IsNet || transportBypassesInterludeLogin;
    }
}
