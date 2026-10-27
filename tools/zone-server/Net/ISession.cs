namespace Fineage.ZoneServer.Net;

/// <summary>
/// Transport-agnostic player session. Separates identity/state from the
/// underlying connection so that gateway routing, reconnection, and testing
/// can swap transports without touching game logic.
/// </summary>
public interface ISession
{
    int SessionId { get; }
    string Account { get; }
    bool Authenticated { get; }
    bool InWorld { get; }

    /// <summary>Authoritative entity object-id assigned by the zone.</summary>
    int EntityId { get; }

    /// <summary>L2 world position (authoritative).</summary>
    int X { get; }
    int Y { get; }
    int Z { get; }

    /// <summary>Send a framed MessagePack message to the client.</summary>
    ValueTask SendAsync<T>(ushort msgType, T payload, CancellationToken ct);

    /// <summary>Graceful disconnect with a reason logged server-side.</summary>
    void Disconnect(string reason);
}
