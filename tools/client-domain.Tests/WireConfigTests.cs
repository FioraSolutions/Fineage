using Xunit;

namespace Fineage.Client.Domain.Tests;

/// <summary>
/// <see cref="FineageWireConfig"/> now only chooses hosts — Net is the sole wire.
///
/// <para>
/// It used to be a wire selector with an <c>Interlude</c> default that could never be
/// observed, because every entry point set Net straight away. These tests pin the two
/// things that actually matter: a profile writes the hosts it was given, and a public
/// profile never resolves to loopback.
/// </para>
/// </summary>
public class WireConfigTests
{
    [Fact]
    public void Playtest_backend_keeps_the_public_tunnel_hosts()
    {
        FineageWireConfig.EnablePlaytestBackend(
            PlaytestEndpoint.DefaultLoginHost,
            PlaytestEndpoint.DefaultLoginPort,
            PlaytestEndpoint.DefaultGameHost,
            PlaytestEndpoint.DefaultGamePort);

        Assert.Equal(PlaytestEndpoint.DefaultLoginHost, FineageWireConfig.LoginHost);
        Assert.Equal(PlaytestEndpoint.DefaultLoginPort, FineageWireConfig.LoginPort);
        Assert.Equal(PlaytestEndpoint.DefaultGameHost, FineageWireConfig.ZoneHost);
        Assert.Equal(PlaytestEndpoint.DefaultGamePort, FineageWireConfig.ZonePort);

        // The regression: a public profile must never resolve to loopback.
        Assert.True(FineageWireConfig.IsPublicLoginHost(FineageWireConfig.LoginHost));
        Assert.True(FineageWireConfig.IsPublicLoginHost(FineageWireConfig.ZoneHost));
    }

    [Fact]
    public void Playtest_backend_falls_back_to_defaults_only_for_empty_hosts()
    {
        FineageWireConfig.EnablePlaytestBackend("   ", 0, null, 0);

        Assert.Equal(PlaytestEndpoint.DefaultLoginHost, FineageWireConfig.LoginHost);
        Assert.Equal(PlaytestEndpoint.DefaultLoginPort, FineageWireConfig.LoginPort);
        Assert.Equal(PlaytestEndpoint.DefaultGameHost, FineageWireConfig.ZoneHost);
        Assert.Equal(PlaytestEndpoint.DefaultGamePort, FineageWireConfig.ZonePort);
    }

    [Fact]
    public void Local_backend_points_at_loopback_net_ports()
    {
        FineageWireConfig.EnableLocalNetBackend();

        Assert.Equal("127.0.0.1", FineageWireConfig.LoginHost);
        Assert.Equal(9106, FineageWireConfig.LoginPort);
        Assert.Equal("127.0.0.1", FineageWireConfig.ZoneHost);
        Assert.Equal(9777, FineageWireConfig.ZonePort);
    }

    /// <summary>
    /// Whichever profile ran last wins. Without this a stale local override would leak into a
    /// public build that calls <c>EnablePlaytestBackend</c> at startup.
    /// </summary>
    [Fact]
    public void The_last_profile_applied_wins()
    {
        FineageWireConfig.EnableLocalNetBackend();
        FineageWireConfig.EnablePlaytestBackend(
            PlaytestEndpoint.DefaultLoginHost,
            PlaytestEndpoint.DefaultLoginPort,
            PlaytestEndpoint.DefaultGameHost,
            PlaytestEndpoint.DefaultGamePort);

        Assert.Equal(PlaytestEndpoint.DefaultLoginHost, FineageWireConfig.LoginHost);
        Assert.Equal(PlaytestEndpoint.DefaultGamePort, FineageWireConfig.ZonePort);
        Assert.True(FineageWireConfig.IsPublicLoginHost(FineageWireConfig.LoginHost));
    }

    [Fact]
    public void Loopback_and_localhost_are_not_public_login_hosts()
    {
        Assert.True(FineageWireConfig.IsPublicLoginHost(PlaytestEndpoint.DefaultLoginHost));
        Assert.False(FineageWireConfig.IsPublicLoginHost("127.0.0.1"));
        Assert.False(FineageWireConfig.IsPublicLoginHost("127.0.1.5"));
        Assert.False(FineageWireConfig.IsPublicLoginHost("localhost"));
        Assert.False(FineageWireConfig.IsPublicLoginHost("   "));
    }
}
