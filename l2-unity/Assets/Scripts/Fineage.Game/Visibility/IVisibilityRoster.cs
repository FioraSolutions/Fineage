using UnityEngine;

/// <summary>
/// Roster <c>World</c> uses to register and drop visible actors (#131 S38).
/// </summary>
public interface IVisibilityRoster
{
    void Register(int entityId, Transform root, bool isLocalPlayer = false);
    void Unregister(int entityId);
}
