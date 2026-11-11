using UnityEngine;

/// <summary>
/// Cycle-safe world entity resolve for Di/FX (#131).
/// Returns transforms only — ACS <c>Entity</c> stays behind <c>World</c>.
/// </summary>
public interface IWorldEntityLookup
{
    Transform FindTransformNoLock(int objectId);
}
