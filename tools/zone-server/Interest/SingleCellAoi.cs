namespace Fineage.ZoneServer.Interest;

/// <summary>
/// Trivial AOI: every entity sees every other entity in the zone.
/// Correct for small player counts (&lt;50). Drop-in replacement for
/// <c>GridAoiProvider</c> when spatial filtering is needed.
/// </summary>
public sealed class SingleCellAoi : IAoiProvider
{
    readonly HashSet<int> _entities = new();
    readonly object _lock = new();

    public void Enter(int entityId, int x, int y)
    {
        lock (_lock) _entities.Add(entityId);
    }

    public void Move(int entityId, int x, int y)
    {
        // No-op: single cell, position doesn't affect visibility.
    }

    public void Leave(int entityId)
    {
        lock (_lock) _entities.Remove(entityId);
    }

    public IReadOnlyCollection<int> GetVisible(int entityId)
    {
        lock (_lock)
        {
            var visible = new List<int>(_entities.Count);
            foreach (int id in _entities)
            {
                if (id != entityId)
                    visible.Add(id);
            }
            return visible;
        }
    }

    /// <summary>Total tracked entities. For metrics.</summary>
    public int Count { get { lock (_lock) return _entities.Count; } }
}
