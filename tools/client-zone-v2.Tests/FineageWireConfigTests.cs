using Xunit;

namespace Fineage.Client.ZoneV2.Tests;

public class FineageWireConfigTests
{
    public FineageWireConfigTests()
    {
        FineageWireConfig.ClearOverride();
        FineageWireConfig.UseLoginHandoff = false;
    }

    [Fact]
    public void Playtest_backend_uses_public_hosts_and_login_handoff()
    {
        FineageWireConfig.EnablePlaytestBackend(null, 0, " ", -1);

        Assert.True(FineageWireConfig.IsZoneV2);
        Assert.True(FineageWireConfig.UseLoginHandoff);
        Assert.Equal(PlaytestEndpoint.DefaultLoginHost, FineageWireConfig.LoginHost);
        Assert.Equal(PlaytestEndpoint.DefaultLoginPort, FineageWireConfig.LoginPort);
        Assert.Equal(PlaytestEndpoint.DefaultGameHost, FineageWireConfig.ZoneHost);
        Assert.Equal(PlaytestEndpoint.DefaultGamePort, FineageWireConfig.ZonePort);
        Assert.Equal("login.fineage.evpoint.by", FineageWireConfig.LoginHost);
        Assert.Equal(2106, FineageWireConfig.LoginPort);
        Assert.Equal("game.fineage.evpoint.by", FineageWireConfig.ZoneHost);
        Assert.Equal(7777, FineageWireConfig.ZonePort);
    }

    [Fact]
    public void Playtest_backend_keeps_explicit_hosts()
    {
        FineageWireConfig.EnablePlaytestBackend(" login.example ", 2106, " game.example ", 7777);

        Assert.Equal("login.example", FineageWireConfig.LoginHost);
        Assert.Equal(2106, FineageWireConfig.LoginPort);
        Assert.Equal("game.example", FineageWireConfig.ZoneHost);
        Assert.Equal(7777, FineageWireConfig.ZonePort);
        Assert.True(FineageWireConfig.IsZoneV2);
        Assert.True(FineageWireConfig.UseLoginHandoff);
    }

    [Fact]
    public void Login_press_starts_zone_session_when_playtest_handoff_is_on()
    {
        FineageWireConfig.EnablePlaytestBackend(null, 0, null, 0);

        Assert.True(FineageWireConfig.LoginPressStartsZoneSession(transportBypassesInterludeLogin: false));
        Assert.Equal("login.fineage.evpoint.by", FineageWireConfig.LoginHost);
        Assert.Equal(2106, FineageWireConfig.LoginPort);
        Assert.NotEqual("127.0.0.1", FineageWireConfig.LoginHost);
        Assert.NotEqual("127.0.0.1", FineageWireConfig.ZoneHost);
    }

    [Fact]
    public void Login_press_starts_zone_session_from_handoff_or_transport_alone()
    {
        FineageWireConfig.UseLoginHandoff = true;
        Assert.False(FineageWireConfig.IsZoneV2);
        Assert.True(FineageWireConfig.LoginPressStartsZoneSession(transportBypassesInterludeLogin: false));

        FineageWireConfig.UseLoginHandoff = false;
        Assert.True(FineageWireConfig.LoginPressStartsZoneSession(transportBypassesInterludeLogin: true));
    }

    [Fact]
    public void Login_press_stays_on_interlude_connect_when_zone_flags_are_off()
    {
        Assert.False(FineageWireConfig.UseLoginHandoff);
        Assert.False(FineageWireConfig.IsZoneV2);
        Assert.False(FineageWireConfig.LoginPressStartsZoneSession(transportBypassesInterludeLogin: false));
    }

    [Fact]
    public void Playtest_backend_replaces_loopback_hosts()
    {
        FineageWireConfig.EnablePlaytestBackend("127.0.0.1", 9106, "127.0.0.1", 9777);
        FineageWireConfig.UseLoginHandoff = false;
        FineageWireConfig.EnablePlaytestBackend(null, 0, null, 0);

        Assert.Equal(PlaytestEndpoint.DefaultLoginHost, FineageWireConfig.LoginHost);
        Assert.Equal(PlaytestEndpoint.DefaultLoginPort, FineageWireConfig.LoginPort);
        Assert.Equal(PlaytestEndpoint.DefaultGameHost, FineageWireConfig.ZoneHost);
        Assert.Equal(PlaytestEndpoint.DefaultGamePort, FineageWireConfig.ZonePort);
        Assert.True(FineageWireConfig.UseLoginHandoff);
        Assert.True(FineageWireConfig.IsZoneV2);
    }
}
