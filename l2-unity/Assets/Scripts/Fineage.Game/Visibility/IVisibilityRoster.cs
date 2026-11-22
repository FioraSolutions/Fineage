using UnityEngine;

/// <summary>
/// Roster <c>World</c> injects to register and drop visible actors (#131 S38, S38h).
/// <c>GameLifetimeScope</c> registers <c>VisibilityManager</c> as this interface.
/// </summary>
public interface IVisibilityRoster
{
    void Register(int entityId, Transform root, bool isLocalPlayer = false);
    void Unregister(int entityId);
}
