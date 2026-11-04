using System;
using UnityEngine;

/// <summary>
/// Feature flag for Interlude stub vs Fineage ZoneServer v2.
/// Public playtest hosts come from <see cref="PlaytestEndpoint"/> defaults.
/// </summary>
public static class FineageWireConfig
{
    public const string PrefKey = "Fineage.WireMode";

    public enum Mode
    {
        Interlude = 0,
        ZoneV2 = 1,
        /// <summary>Parallel Fineage Net (PacketEnvelope) on forwarded ports 9106/9777 by default.</summary>
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
    /// ZoneV2 login handoff for Fineage-Backend. Empty hosts use the public playtest endpoints.
    /// Interlude Blowfish login hangs on this TCP service.
    /// </summary>
    public static void EnablePlaytestBackend(
        string loginHost,
        int loginPort,
        string zoneHost,
        int zonePort)
    {
#if FINEAGE_V2_WIRE
        SetOverride(Mode.ZoneV2);
        LoginHost = string.IsNullOrWhiteSpace(loginHost) ? PlaytestEndpoint.DefaultLoginHost : loginHost.Trim();
        LoginPort = loginPort > 0 ? loginPort : PlaytestEndpoint.DefaultLoginPort;
        ZoneHost = string.IsNullOrWhiteSpace(zoneHost) ? PlaytestEndpoint.DefaultGameHost : zoneHost.Trim();
        ZonePort = zonePort > 0 ? zonePort : PlaytestEndpoint.DefaultGamePort;
        UseLoginHandoff = true;
#else
        // V2 MessagePack playtest path disabled — fall back to local Net.
        EnableLocalNetBackend();
#endif
    }

    /// <summary>
    /// Local Fineage Net path (PacketEnvelope). Requires backend <c>FINEAGE_NET_ENABLE=1</c>
    /// on the forwarded ports 9106/9777 (legacy V2 sits on 9206/9877).
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
        get
        {
#if FINEAGE_V2_WIRE
            return Current == Mode.ZoneV2;
#else
            // V2 MessagePack entry points are compile-time off; Net is the live path.
            return false;
#endif
        }
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
    /// Log In must call zone <c>BeginSession</c> when ZoneV2, login handoff, or the
    /// transport bypasses Interlude — even if <c>LoginClient</c> is already connected.
    /// </summary>
    public static bool LoginPressStartsZoneSession(bool transportBypassesInterludeLogin)
    {
        return UseLoginHandoff || IsZoneV2 || IsNet || transportBypassesInterludeLogin;
    }
}

