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
/// Interlude socket is unreachable. What remains here is the host/port pair that both
/// profiles write, and the local loopback constants.
/// </para>
///
/// <para>
/// Which pair wins is <see cref="FineageServerProfile"/>'s call: remote playtest by
/// default, local Fineage-Backend on request. A release-clients build never asks, so it
/// always dials the public tunnel.
/// </para>
/// </summary>
public static class FineageWireConfig
{
    /// <summary>Fineage-Backend run on this machine (see Backend <c>scripts/run-local-net.sh</c>).</summary>
    public const string LocalHost = "127.0.0.1";
    public const int LocalLoginPort = 9106;
    public const int LocalGamePort = 9777;

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
        string loginHost = LocalHost,
        int loginPort = LocalLoginPort,
        string zoneHost = LocalHost,
        int zonePort = LocalGamePort)
    {
        LoginHost = string.IsNullOrWhiteSpace(loginHost) ? LocalHost : loginHost.Trim();
        LoginPort = loginPort > 0 ? loginPort : LocalLoginPort;
        ZoneHost = string.IsNullOrWhiteSpace(zoneHost) ? LocalHost : zoneHost.Trim();
        ZonePort = zonePort > 0 ? zonePort : LocalGamePort;
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
