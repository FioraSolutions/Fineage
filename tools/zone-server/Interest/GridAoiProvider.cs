namespace Fineage.ZoneServer.Interest;

/// <summary>
/// Spatial-hash AOI: 3×3 cell neighborhood (~Interlude visibility at 3000 L2 units/cell).
/// </summary>
public sealed class GridAoiProvider : IAoiProvider
{
    public const int CellSize = 3000;

    readonly Dictionary<long, HashSet<int>> _cellToEntities = new();
    readonly Dictionary<int, long> _entityToCell = new();
    readonly object _lock = new();

    public static long PackCell(int gridX, int gridY) =>
        ((long)gridX << 32) | (uint)gridY;

    public static int CellCoord(int worldUnits) =>
        (int)Math.Floor((double)worldUnits / CellSize);

    public void Enter(int entityId, int x, int y)
    {
        long cell = PackCell(CellCoord(x), CellCoord(y));
        lock (_lock)
        {
            if (_entityToCell.ContainsKey(entityId))
                RemoveFromCell(entityId);
            AddToCell(entityId, cell);
        }
    }

    public void Move(int entityId, int x, int y)
    {
        long cell = PackCell(CellCoord(x), CellCoord(y));
        lock (_lock)
        {
            if (!_entityToCell.TryGetValue(entityId, out long current) || current == cell)
                return;
            RemoveFromCell(entityId);
            AddToCell(entityId, cell);
        }
    }

    public void Leave(int entityId)
    {
        lock (_lock)
            RemoveFromCell(entityId);
    }

    public IReadOnlyCollection<int> GetVisible(int entityId)
    {
        lock (_lock)
        {
            if (!_entityToCell.TryGetValue(entityId, out long cellKey))
                return Array.Empty<int>();

            int gx = (int)(cellKey >> 32);
            int gy = (int)(uint)cellKey;
            var visible = new List<int>();
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    long neighbor = PackCell(gx + dx, gy + dy);
                    if (!_cellToEntities.TryGetValue(neighbor, out HashSet<int>? set))
                        continue;
                    foreach (int id in set)
                    {
                        if (id != entityId)
                            visible.Add(id);
                    }
                }
            }

            return visible;
        }
    }

    /// <summary>Total tracked entities. For metrics/tests.</summary>
    public int Count
    {
        get
        {
            lock (_lock)
                return _entityToCell.Count;
        }
    }

    void AddToCell(int entityId, long cell)
    {
        if (!_cellToEntities.TryGetValue(cell, out HashSet<int>? set))
        {
            set = new HashSet<int>();
            _cellToEntities[cell] = set;
        }

        set.Add(entityId);
        _entityToCell[entityId] = cell;
    }

    void RemoveFromCell(int entityId)
    {
        if (!_entityToCell.TryGetValue(entityId, out long cell))
            return;

        if (_cellToEntities.TryGetValue(cell, out HashSet<int>? set))
        {
            set.Remove(entityId);
            if (set.Count == 0)
                _cellToEntities.Remove(cell);
        }

        _entityToCell.Remove(entityId);
    }
}
