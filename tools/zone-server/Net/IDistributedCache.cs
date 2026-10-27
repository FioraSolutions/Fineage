namespace Fineage.ZoneServer.Net;

/// <summary>
/// Simple key-value cache abstraction. In-memory for local dev;
/// swap to Redis (via StackExchange.Redis or similar) for multi-process.
/// 
/// Used for: session directory, cross-zone presence, guild online list.
/// NOT used for: per-tick entity state (too slow for hot path).
/// </summary>
public interface IDistributedCache
{
    ValueTask<T?> GetAsync<T>(string key, CancellationToken ct = default) where T : class;
    ValueTask SetAsync<T>(string key, T value, TimeSpan? ttl = null, CancellationToken ct = default) where T : class;
    ValueTask RemoveAsync(string key, CancellationToken ct = default);
}
