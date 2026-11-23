using System;

/// <summary>
/// Query over the same <see cref="WorldEntityRegistry{T}"/> <c>World</c> mutates.
/// Registered as <see cref="ISpawnedEntities"/> so target scans do not depend on <c>World</c>.
/// </summary>
public sealed class SpawnedEntityCatalog : ISpawnedEntities
{
    readonly WorldEntityRegistry<Entity> _entities;

    public SpawnedEntityCatalog(WorldEntityRegistry<Entity> entities)
    {
        if (entities == null)
        {
            throw new InvalidOperationException(
                "[SpawnedEntityCatalog] WorldEntityRegistry<Entity> missing — GameLifetimeScope did not register the spawned entity registry, so targets were not scanned.");
        }

        _entities = entities;
    }

    public Entity Find(int objectId)
    {
        return _entities.GetOrNull(objectId);
    }

    public void ForEach(Action<Entity> visit)
    {
        _entities.ForEach(visit);
    }
}
