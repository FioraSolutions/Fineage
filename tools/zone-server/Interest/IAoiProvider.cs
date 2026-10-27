namespace Fineage.ZoneServer.Interest;

/// <summary>
/// Area-of-Interest provider. Given an entity's position, returns
/// the set of entity IDs visible to it.
/// 
/// <para>
/// MVP: <see cref="SingleCellAoi"/> — everyone sees everyone (flat list).
/// Next: <c>GridAoiProvider</c> — spatial hash grid with 3×3 cell neighborhood.
/// Later: cross-zone ghost entities at zone boundaries.
/// </para>
/// </summary>
public interface IAoiProvider
{
    /// <summary>Notify that an entity entered the world at the given position.</summary>
    void Enter(int entityId, int x, int y);

    /// <summary>Update an entity's position (may change cell membership).</summary>
    void Move(int entityId, int x, int y);

    /// <summary>Remove an entity from the world.</summary>
    void Leave(int entityId);

    /// <summary>
    /// Get all entity IDs visible to the given entity.
    /// Result does NOT include the queried entity itself.
    /// </summary>
    IReadOnlyCollection<int> GetVisible(int entityId);
}
