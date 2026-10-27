using System.Collections.Concurrent;

namespace Fineage.ZoneServer.Net;

/// <summary>
/// In-memory session store for single-process ZoneServer.
/// Thread-safe via ConcurrentDictionary. Replace with Redis-backed
/// implementation when gateway routing is introduced.
/// </summary>
public sealed class MemorySessionStore : ISessionStore
{
    readonly ConcurrentDictionary<int, ISession> _byId = new();
    readonly ConcurrentDictionary<string, ISession> _byAccount = new(StringComparer.OrdinalIgnoreCase);

    public bool TryAdd(ISession session)
    {
        if (!_byId.TryAdd(session.SessionId, session))
            return false;

        _byAccount[session.Account] = session;
        return true;
    }

    public bool TryRemove(int sessionId)
    {
        if (!_byId.TryRemove(sessionId, out var session))
            return false;

        _byAccount.TryRemove(session.Account, out _);
        return true;
    }

    public ISession? Get(int sessionId)
    {
        _byId.TryGetValue(sessionId, out var session);
        return session;
    }

    public ISession? GetByAccount(string account)
    {
        _byAccount.TryGetValue(account, out var session);
        return session;
    }

    public IReadOnlyCollection<ISession> All => _byId.Values.ToArray();

    public int Count => _byId.Count;
}
