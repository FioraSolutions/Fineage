using Xunit;

namespace Fineage.Client.Domain.Tests;

public class ZoneConnectionStateTests
{
    [Theory]
    [InlineData(false, false, false, false, false, ZoneConnectionState.Disconnected)]
    [InlineData(true, false, false, false, false, ZoneConnectionState.Connecting)]
    [InlineData(true, true, false, false, false, ZoneConnectionState.Connecting)]
    [InlineData(false, true, true, false, false, ZoneConnectionState.Connected)]
    [InlineData(false, true, true, true, false, ZoneConnectionState.Authenticating)]
    [InlineData(false, true, true, true, true, ZoneConnectionState.InWorld)]
    [InlineData(false, true, true, false, true, ZoneConnectionState.InWorld)]
    [InlineData(false, true, false, true, true, ZoneConnectionState.Disconnected)]
    public void Resolve_maps_flags_to_lifecycle(
        bool connecting,
        bool hasTransport,
        bool socketConnected,
        bool helloOk,
        bool inWorld,
        ZoneConnectionState expected)
    {
        Assert.Equal(
            expected,
            ZoneConnectionStateResolver.Resolve(
                connecting,
                hasTransport,
                socketConnected,
                helloOk,
                inWorld));
    }

    [Theory]
    [InlineData(false, false, false)]
    [InlineData(true, false, false)]
    [InlineData(false, true, false)]
    [InlineData(true, true, true)]
    public void IsSocketConnected_requires_transport_and_tcp(
        bool hasTransport,
        bool socketConnected,
        bool expected)
    {
        Assert.Equal(
            expected,
            ZoneConnectionStateResolver.IsSocketConnected(hasTransport, socketConnected));
    }
}
