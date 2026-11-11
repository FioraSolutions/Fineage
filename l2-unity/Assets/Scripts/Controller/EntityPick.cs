using UnityEngine;

/// <summary>
/// Mouse pick SoT (#930): entity resolve uses <c>EntityClick</c> only.
/// CharacterController on Entity must never win the first ray — fat monster rigs
/// otherwise swallow clicks meant for <c>click_area</c>.
/// </summary>
public static class EntityPick
{
    public const float DefaultMaxDistance = 10000f;

    static LayerMask EntityClickMask => LayerMask.GetMask("EntityClick");

    /// <summary>
    /// Raycast entity click colliders and resolve <see cref="Entity"/> on the root.
    /// </summary>
    public static bool TryPick(
        Camera cam,
        Vector2 screenPos,
        float maxDistance,
        out Entity entity,
        out RaycastHit hit)
    {
        entity = null;
        hit = default;
        if (cam == null)
            return false;

        Ray ray = cam.ScreenPointToRay(screenPos);
        return TryPick(ray, maxDistance, out entity, out hit);
    }

    public static bool TryPick(
        Ray ray,
        float maxDistance,
        out Entity entity,
        out RaycastHit hit)
    {
        entity = null;
        hit = default;
        if (!Physics.Raycast(ray, out hit, maxDistance, EntityClickMask))
            return false;

        entity = hit.collider != null
            ? hit.collider.GetComponentInParent<Entity>()
            : null;
        return entity != null;
    }

    /// <summary>
    /// Ground / move ray: everything except click-through layers. Call only after entity pick misses.
    /// </summary>
    public static bool TryPickMove(
        Ray ray,
        float maxDistance,
        LayerMask clickThroughMask,
        out RaycastHit hit)
    {
        return Physics.Raycast(ray, out hit, maxDistance, ~clickThroughMask);
    }
}
