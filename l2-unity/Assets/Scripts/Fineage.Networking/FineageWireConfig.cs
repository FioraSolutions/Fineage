using System;

/// <summary>
/// Where the client dials Fineage Net, and nothing else.
///
/// <para>
/// This used to be a wire <i>selector</i>: an <c>Interlude</c> mode (Blowfish login stub plus
/// the Interlude game socket) against <c>Net</c>, persisted in PlayerPrefs and defaulting to
/// Interlude. Every entry point then set Net immediately, so the selector had one reachable
/// value and a default that could never be observed — except by an <c>else</c> branch, which
/// would silently do nothing.
/// </para>
///
/// <para>
/// Net is now the only wire. Legacy MessagePack Zone path is gone; Fineage Net only. The
/// Interlude socket is unreachable: no build sets <c>FINEAGE_NET_CLIENT</c>, and
/// <see cref="FineagePublicMacBuild"/> ships the public Net playtest. What remains here is
/// the host/port pair that both profiles write.
/// </para>
/// </summary>
public static class FineageWireConfig
{
    public static string ZoneHost = PlaytestEndpoint.DefaultGameHost;
    public static int ZonePort = PlaytestEndpoint.DefaultGamePort;

    /// <summary>Fineage-Backend login TCP. A handoff token from this service is the zone password.</summary>
    public static string LoginHost = PlaytestEndpoint.DefaultLoginHost;
    public static int LoginPort = PlaytestEndpoint.DefaultLoginPort;

    /// <summary>
    /// Fineage Net against the public playtest tunnel.
    /// Empty hosts fall back to the <see cref="PlaytestEndpoint"/> defaults.
    /// </summary>
    public static void EnablePlaytestBackend(
        string loginHost,
        int loginPort,
        string zoneHost,
        int zonePort)
    {
        LoginHost = string.IsNullOrWhiteSpace(loginHost) ? PlaytestEndpoint.DefaultLoginHost : loginHost.Trim();
        LoginPort = loginPort > 0 ? loginPort : PlaytestEndpoint.DefaultLoginPort;
        ZoneHost = string.IsNullOrWhiteSpace(zoneHost) ? PlaytestEndpoint.DefaultGameHost : zoneHost.Trim();
        ZonePort = zonePort > 0 ? zonePort : PlaytestEndpoint.DefaultGamePort;
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
        LoginHost = string.IsNullOrWhiteSpace(loginHost) ? "127.0.0.1" : loginHost.Trim();
        LoginPort = loginPort > 0 ? loginPort : 9106;
        ZoneHost = string.IsNullOrWhiteSpace(zoneHost) ? "127.0.0.1" : zoneHost.Trim();
        ZonePort = zonePort > 0 ? zonePort : 9777;
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
}
