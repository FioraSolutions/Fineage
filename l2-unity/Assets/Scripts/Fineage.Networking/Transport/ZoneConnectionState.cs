/// <summary>
/// Zone TCP / session lifecycle for <see cref="IZoneTransport"/>.
/// Distinct from "a <c>NetSession</c> MonoBehaviour exists" — that is <see cref="IZoneTransport.HasSession"/>.
/// </summary>
public enum ZoneConnectionState
{
    /// <summary>No live zone socket (or transport disposed).</summary>
    Disconnected = 0,

    /// <summary>ConnectAsync in flight; socket not yet usable.</summary>
    Connecting = 1,

    /// <summary>TCP up; Hello / HelloAck not finished.</summary>
    Connected = 2,

    /// <summary>HelloAck received; ZoneAuth / EnterWorld in progress.</summary>
    Authenticating = 3,

    /// <summary>Local player EntitySpawn received (SessionObjectId set).</summary>
    InWorld = 4,
}
