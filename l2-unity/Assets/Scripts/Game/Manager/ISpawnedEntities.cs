using System;

/// <summary>
/// Spawned actors <c>TargetManager</c> scans (#131).
/// This is not <c>World</c>: <c>World</c> injects <c>ClickManager</c>, and
/// <c>ClickManager</c> injects <c>TargetManager</c>, so the scan must not take <c>World</c>.
/// </summary>
public interface ISpawnedEntities
{
    Entity Find(int objectId);

    void ForEach(Action<Entity> visit);
}
