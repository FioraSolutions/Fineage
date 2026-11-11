/// <summary>
/// Pure mapping from NetSession zone flags → <see cref="ZoneConnectionState"/>.
/// Kept engine-free so tools CI can lock the transitions without Unity.
/// </summary>
public static class ZoneConnectionStateResolver
{
    public static ZoneConnectionState Resolve(
        bool connecting,
        bool hasTransport,
        bool socketConnected,
        bool helloOk,
        bool inWorld)
    {
        if (connecting && (!hasTransport || !socketConnected))
            return ZoneConnectionState.Connecting;

        if (!hasTransport || !socketConnected)
            return ZoneConnectionState.Disconnected;

        if (inWorld)
            return ZoneConnectionState.InWorld;

        if (helloOk)
            return ZoneConnectionState.Authenticating;

        return ZoneConnectionState.Connected;
    }

    /// <summary>
    /// True when the zone TCP socket is up. Does not imply auth or InWorld.
    /// </summary>
    public static bool IsSocketConnected(bool hasTransport, bool socketConnected) =>
        hasTransport && socketConnected;
}
