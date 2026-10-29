#nullable disable
using System;

/// <summary>Which backend the client talks to. Public RP is the default; Local is opt-in only.</summary>
public enum FineageBackend
{
    Public = 0,
    Local = 1
}

/// <summary>An immutable TCP destination.</summary>
public readonly struct HostPort
{
    public readonly string Host;
    public readonly int Port;

    public HostPort(string host, int port)
    {
        Host = host;
        Port = port;
    }

    public bool IsValid
    {
        get { return !string.IsNullOrWhiteSpace(Host) && Port >= 1 && Port <= 65535; }
    }

    public override string ToString()
    {
        return Host + ":" + Port;
    }
}

/// <summary>
/// Single source of truth for where the client connects: Fineage-Backend login TCP and zone TCP.
/// Built once at startup (see <c>FineageConnection.Initialize</c>) and never mutated afterwards.
///
/// Invariant: a <see cref="FineageBackend.Public"/> instance never contains a loopback or
/// unspecified address, and <see cref="ZoneForHandoff"/> never returns one either.
/// </summary>
public sealed class FineageEndpoints
{
    public const string LocalHost = "127.0.0.1";
    public const int LocalLoginPort = 9106;
    public const int LocalZonePort = 9777;

    public FineageBackend Backend { get; }
    public HostPort Login { get; }
    public HostPort Zone { get; }

    public bool IsPublic
    {
        get { return Backend == FineageBackend.Public; }
    }

    FineageEndpoints(FineageBackend backend, HostPort login, HostPort zone)
    {
        Backend = backend;
        Login = login;
        Zone = zone;
    }

    /// <summary>The shipped public RP: login.fineage.evpoint.by:2106 and game.fineage.evpoint.by:7777.</summary>
    public static FineageEndpoints PublicDefault()
    {
        return new FineageEndpoints(
            FineageBackend.Public,
            new HostPort(PlaytestEndpoint.DefaultLoginHost, PlaytestEndpoint.DefaultLoginPort),
            new HostPort(PlaytestEndpoint.DefaultGameHost, PlaytestEndpoint.DefaultGamePort));
    }

    /// <summary>
    /// Public RP with optional overrides (baked JSON, sidecar, env, command line).
    /// Any loopback / empty / invalid override falls back to the public default for that field.
    /// </summary>
    public static FineageEndpoints Public(PlaytestEndpoint overrides)
    {
        string loginHost = PublicHostOr(overrides != null ? overrides.LoginHost : null, PlaytestEndpoint.DefaultLoginHost);
        string zoneHost = PublicHostOr(overrides != null ? overrides.GameHost : null, PlaytestEndpoint.DefaultGameHost);
        int loginPort = PortOr(overrides != null ? overrides.LoginPort : 0, PlaytestEndpoint.DefaultLoginPort);
        int zonePort = PortOr(overrides != null ? overrides.GamePort : 0, PlaytestEndpoint.DefaultGamePort);
        return new FineageEndpoints(
            FineageBackend.Public,
            new HostPort(loginHost, loginPort),
            new HostPort(zoneHost, zonePort));
    }

    /// <summary>Local Fineage-Backend. Defaults to 127.0.0.1:9106 / 127.0.0.1:9777.</summary>
    public static FineageEndpoints Local(string loginHost, int loginPort, string zoneHost, int zonePort)
    {
        return new FineageEndpoints(
            FineageBackend.Local,
            new HostPort(TrimOr(loginHost, LocalHost), PortOr(loginPort, LocalLoginPort)),
            new HostPort(TrimOr(zoneHost, LocalHost), PortOr(zonePort, LocalZonePort)));
    }

    /// <summary>
    /// Where to dial the zone after the login server issued a handoff ticket.
    /// The advertised address is used when it is usable from this client; otherwise the
    /// configured zone endpoint is used. On the public RP a backend that advertises its own
    /// loopback / LAN-unspecified address (common behind a tunnel) can never send us to 127.0.0.1.
    /// </summary>
    public HostPort ZoneForHandoff(string advertisedHost, int advertisedPort)
    {
        string host = Clean(advertisedHost);
        if (host == null || (IsPublic && IsLoopbackOrUnspecified(host)))
            host = Zone.Host;
        int port = advertisedPort >= 1 && advertisedPort <= 65535 ? advertisedPort : Zone.Port;
        return new HostPort(host, port);
    }

    public override string ToString()
    {
        return Backend + " login=" + Login + " zone=" + Zone;
    }

    /// <summary>True for 127.0.0.0/8, ::1, 0.0.0.0, :: and "localhost".</summary>
    public static bool IsLoopbackOrUnspecified(string host)
    {
        string h = Clean(host);
        if (h == null)
            return false;
        if (string.Equals(h, "localhost", StringComparison.OrdinalIgnoreCase))
            return true;
        return h.StartsWith("127.", StringComparison.Ordinal)
            || h == "::1" || h == "[::1]" || h == "0.0.0.0" || h == "::";
    }

    static string PublicHostOr(string candidate, string fallback)
    {
        string h = Clean(candidate);
        return h == null || IsLoopbackOrUnspecified(h) ? fallback : h;
    }

    static string TrimOr(string candidate, string fallback)
    {
        return Clean(candidate) ?? fallback;
    }

    static int PortOr(int port, int fallback)
    {
        return port >= 1 && port <= 65535 ? port : fallback;
    }

    static string Clean(string host)
    {
        return string.IsNullOrWhiteSpace(host) ? null : host.Trim();
    }
}
