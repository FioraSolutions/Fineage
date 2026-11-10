#if UNITY_EDITOR || DEVELOPMENT_BUILD
using UnityEngine;

/// <summary>
/// Tracks persistent ground DropMesh (L2 AL2Pickup drop_mesh), not e_u056_a FX.
/// Logging currently disabled — Configure retained for call sites.
/// Console filter: [DropMeshLifetime]
/// </summary>
public sealed class DropMeshLifetimeTracker : MonoBehaviour
{
    public void Configure(int itemObjectId, int itemId, string prefabName, bool coinVisual)
    {
        // no-op while lifetime logging is disabled
    }
}
#endif
