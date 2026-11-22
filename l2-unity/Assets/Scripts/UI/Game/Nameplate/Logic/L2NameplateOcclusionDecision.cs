using UnityEngine;

/// <summary>
/// Pure Interlude-style head-anchor occlusion decision (camera line-of-sight).
/// </summary>
public static class L2NameplateOcclusionDecision
{
    public const float ClearLosEpsilonMeters = 0.15f;

    /// <summary>
    /// Returns true when the head anchor should draw (no blocking hit, or only the owner's colliders).
    /// </summary>
    public static bool IsHeadAnchorVisible(
        float hitDistanceMeters,
        float anchorDistanceMeters,
        Collider hitCollider,
        Transform ownerRoot)
    {
        if (anchorDistanceMeters <= ClearLosEpsilonMeters)
            return true;

        if (hitCollider == null)
            return true;

        if (hitDistanceMeters >= anchorDistanceMeters - ClearLosEpsilonMeters)
            return true;

        if (ownerRoot == null)
            return false;

        Transform hitTransform = hitCollider.transform;
        if (hitTransform == ownerRoot || hitTransform.IsChildOf(ownerRoot))
            return true;

        return false;
    }

    public static bool IsOnScreen(Camera camera, Vector3 worldAnchor)
    {
        if (camera == null)
            return false;

        Vector3 viewPort = camera.WorldToViewportPoint(worldAnchor);
        return viewPort.z > 0f
               && viewPort.x >= 0f && viewPort.x <= 1f
               && viewPort.y >= 0f && viewPort.y <= 1f;
    }
}