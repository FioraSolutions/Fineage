using System.Collections.Concurrent;

namespace Fineage.ZoneServer.Net;

/// <summary>
/// In-memory cache with optional TTL eviction. No external dependencies.
/// Suitable for single-process dev and testing; swap for Redis in production.
/// 
/// Eviction is lazy (checked on read) — good enough for session directory scale.
/// A background sweep can be added if entry count grows large.
/// </summary>
public sealed class MemoryCache : IDistributedCache
{
    readonly record struct CacheEntry(object Value, DateTime? ExpiresAt);

    readonly ConcurrentDictionary<string, CacheEntry> _store = new(StringComparer.Ordinal);

    public ValueTask<T?> GetAsync<T>(string key, CancellationToken ct = default) where T : class
    {
        if (!_store.TryGetValue(key, out var entry))
            return ValueTask.FromResult<T?>(null);

        if (entry.ExpiresAt.HasValue && DateTime.UtcNow >= entry.ExpiresAt.Value)
        {
            _store.TryRemove(key, out _);
            return ValueTask.FromResult<T?>(null);
        }

        return ValueTask.FromResult(entry.Value as T);
    }

    public ValueTask SetAsync<T>(string key, T value, TimeSpan? ttl = null, CancellationToken ct = default) where T : class
    {
        var expires = ttl.HasValue ? DateTime.UtcNow + ttl.Value : (DateTime?)null;
        _store[key] = new CacheEntry(value, expires);
        return ValueTask.CompletedTask;
    }

    public ValueTask RemoveAsync(string key, CancellationToken ct = default)
    {
        _store.TryRemove(key, out _);
        return ValueTask.CompletedTask;
    }

    /// <summary>Number of entries (including possibly expired). For metrics.</summary>
    public int Count => _store.Count;
}
