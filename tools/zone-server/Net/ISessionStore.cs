namespace Fineage.ZoneServer.Net;

/// <summary>
/// Registry of active sessions. In-memory for single-process; replaceable
/// with a Redis-backed directory when gateway routing is added.
/// </summary>
public interface ISessionStore
{
    /// <summary>Register a new session. Returns false if sessionId already exists.</summary>
    bool TryAdd(ISession session);

    /// <summary>Remove a session (disconnect / timeout).</summary>
    bool TryRemove(int sessionId);

    /// <summary>Lookup by session id.</summary>
    ISession? Get(int sessionId);

    /// <summary>Lookup by account name (e.g. for kick-duplicate).</summary>
    ISession? GetByAccount(string account);

    /// <summary>Snapshot of all active sessions — use sparingly (broadcast, metrics).</summary>
    IReadOnlyCollection<ISession> All { get; }

    int Count { get; }
}
