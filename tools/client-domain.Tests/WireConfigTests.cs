using Xunit;

namespace Fineage.Client.Domain.Tests;

/// <summary>
/// Wire selection after the V2 MessagePack mode was retired. The important case is
/// <see cref="FineageWireConfig.EnablePlaytestBackend"/>: a release build calls it with the
/// public tunnel hosts, and it used to silently fall back to <c>127.0.0.1</c> because the
/// body was guarded by the (always undefined) <c>FINEAGE_V2_WIRE</c> define.
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

        Assert.Equal(FineageWireConfig.Mode.Net, FineageWireConfig.Current);
        Assert.True(FineageWireConfig.IsNet);
        Assert.True(FineageWireConfig.UseLoginHandoff);
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

        Assert.True(FineageWireConfig.IsNet);
        Assert.Equal("127.0.0.1", FineageWireConfig.LoginHost);
        Assert.Equal(9106, FineageWireConfig.LoginPort);
        Assert.Equal("127.0.0.1", FineageWireConfig.ZoneHost);
        Assert.Equal(9777, FineageWireConfig.ZonePort);
        Assert.True(FineageWireConfig.UseLoginHandoff);
    }

    [Fact]
    public void Login_press_starts_a_zone_session_when_net_owns_the_login()
    {
        FineageWireConfig.EnableLocalNetBackend();
        Assert.True(FineageWireConfig.LoginPressStartsZoneSession(false));
        Assert.True(FineageWireConfig.LoginPressStartsZoneSession(true));
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
