using Xunit;

namespace Fineage.Client.ZoneV2.Tests;

public class FineageEndpointsTests
{
    [Fact]
    public void Default_is_public_rp()
    {
        FineageEndpoints e = FineageEndpoints.PublicDefault();

        Assert.True(e.IsPublic);
        Assert.Equal("login.fineage.evpoint.by:2106", e.Login.ToString());
        Assert.Equal("game.fineage.evpoint.by:7777", e.Zone.ToString());
    }

    [Fact]
    public void Public_without_overrides_matches_default()
    {
        FineageEndpoints e = FineageEndpoints.Public(PlaytestEndpoint.Resolve(new PlaytestEndpointInput()));

        Assert.Equal("login.fineage.evpoint.by:2106", e.Login.ToString());
        Assert.Equal("game.fineage.evpoint.by:7777", e.Zone.ToString());
    }

    [Fact]
    public void Public_keeps_explicit_public_overrides()
    {
        var input = new PlaytestEndpointInput
        {
            CommandLine = new[] { "-fineageLoginHost", " login.example ", "-fineageGameHost=game.example", "-fineageGamePort=7000" },
        };

        FineageEndpoints e = FineageEndpoints.Public(PlaytestEndpoint.Resolve(input));

        Assert.Equal("login.example:2106", e.Login.ToString());
        Assert.Equal("game.example:7000", e.Zone.ToString());
    }

    [Theory]
    [InlineData("127.0.0.1")]
    [InlineData("127.1.2.3")]
    [InlineData("localhost")]
    [InlineData("0.0.0.0")]
    [InlineData("::1")]
    public void Public_rejects_loopback_overrides(string host)
    {
        var input = new PlaytestEndpointInput { CommandLine = new[] { "-fineageLoginHost", host, "-fineageGameHost", host } };

        FineageEndpoints e = FineageEndpoints.Public(PlaytestEndpoint.Resolve(input));

        Assert.Equal(PlaytestEndpoint.DefaultLoginHost, e.Login.Host);
        Assert.Equal(PlaytestEndpoint.DefaultGameHost, e.Zone.Host);
    }

    [Fact]
    public void Local_defaults_to_loopback_dev_ports()
    {
        FineageEndpoints e = FineageEndpoints.Local(null, 0, " ", -1);

        Assert.Equal(FineageBackend.Local, e.Backend);
        Assert.Equal("127.0.0.1:9106", e.Login.ToString());
        Assert.Equal("127.0.0.1:9777", e.Zone.ToString());
    }

    [Fact]
    public void Public_handoff_never_dials_loopback()
    {
        FineageEndpoints e = FineageEndpoints.PublicDefault();

        Assert.Equal(PlaytestEndpoint.DefaultGameHost, e.ZoneForHandoff("127.0.0.1", 9777).Host);
        Assert.Equal(PlaytestEndpoint.DefaultGameHost, e.ZoneForHandoff("localhost", 0).Host);
        Assert.Equal(7777, e.ZoneForHandoff("", 0).Port);
    }

    [Fact]
    public void Public_handoff_honours_advertised_public_zone()
    {
        HostPort zone = FineageEndpoints.PublicDefault().ZoneForHandoff("zone2.fineage.evpoint.by", 7800);

        Assert.Equal("zone2.fineage.evpoint.by:7800", zone.ToString());
    }

    [Fact]
    public void Local_handoff_allows_loopback()
    {
        HostPort zone = FineageEndpoints.Local(null, 0, null, 0).ZoneForHandoff("127.0.0.1", 9778);

        Assert.Equal("127.0.0.1:9778", zone.ToString());
    }

    [Fact]
    public void Wire_config_use_selects_zone_v2_and_endpoints()
    {
        FineageWireConfig.ClearOverride();
        FineageEndpoints local = FineageEndpoints.Local(null, 0, null, 0);

        FineageWireConfig.Use(local);

        Assert.True(FineageWireConfig.IsZoneV2);
        Assert.Same(local, FineageWireConfig.Endpoints);

        FineageWireConfig.Use(FineageEndpoints.PublicDefault());
        Assert.True(FineageWireConfig.Endpoints.IsPublic);
    }

    [Fact]
    public void Server_list_has_one_online_world_behind_the_zone()
    {
        var servers = LobbyServer.FromEndpoints(FineageEndpoints.PublicDefault(), 2);

        LobbyServer only = Assert.Single(servers);
        Assert.True(only.Online);
        Assert.Equal(2, only.CharacterCount);
        Assert.Equal("game.fineage.evpoint.by:7777", only.Zone.ToString());
    }
}
