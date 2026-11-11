using UnityEngine;

public sealed partial class ItemDropPicker
{
    public ItemDropPicker(ItemDropLayerService layers)
    {
        _layers = layers;
    }

    public bool TryPick(
        Ray ray,
        float distance,
        ItemEntity sticky,
        out ItemEntity item,
        out float hitDistance)
    {
        item = null;
        hitDistance = 0f;
        LayerMask mask = _layers.Mask;
        if (mask == 0)
            return false;

        if (sticky != null && !sticky)
            sticky = null;

        float maxDist = Mathf.Min(distance, MaxDistance);
        bool hasGround = TryGetGroundPoint(ray, maxDist, out Vector3 groundPoint);

        if (sticky != null &&
            TryItemHoverDistance(sticky, ray, maxDist, hasGround, groundPoint, out float stickyDist) &&
            stickyDist <= KeepRadius)
        {
            item = sticky;
            hitDistance = AlongRay(ray, sticky.transform.position);
            return true;
        }

        ItemEntity best = null;
        float bestRadial = float.MaxValue;
        float bestAlong = float.MaxValue;
        Collect(
            Physics.OverlapCapsuleNonAlloc(
                ray.origin,
                ray.origin + ray.direction * maxDist,
                CastRadius,
                OverlapBuffer,
                mask,
                QueryTriggerInteraction.Collide),
            ray,
            maxDist,
            CastRadius,
            hasGround,
            groundPoint,
            ref best,
            ref bestRadial,
            ref bestAlong);

        if (hasGround)
        {
            Collect(
                Physics.OverlapSphereNonAlloc(
                    groundPoint,
                    CastRadius,
                    OverlapBuffer,
                    mask,
                    QueryTriggerInteraction.Collide),
                ray,
                maxDist,
                CastRadius,
                hasGround,
                groundPoint,
                ref best,
                ref bestRadial,
                ref bestAlong);
        }

        if (best == null)
            return false;

        item = best;
        hitDistance = bestAlong;
        return true;
    }

    bool TryGetGroundPoint(Ray ray, float distance, out Vector3 point)
    {
        point = default;
        int mask = GroundPickMask();
        if (mask == 0)
            return false;
        if (!Physics.Raycast(ray, out RaycastHit hit, distance, mask, QueryTriggerInteraction.Ignore))
            return false;
        point = hit.point;
        return true;
    }

    int GroundPickMask()
    {
        if (_groundPickMask >= 0)
            return _groundPickMask;

        _groundPickMask = 0;
        AddLayer(ref _groundPickMask, "Terrain");
        AddLayer(ref _groundPickMask, "StaticMesh");
        AddLayer(ref _groundPickMask, "Brush");
        AddLayer(ref _groundPickMask, "Default");
        AddLayer(ref _groundPickMask, "Unwalkable");
        AddLayer(ref _groundPickMask, "AllowWalk");
        AddLayer(ref _groundPickMask, "Obstacle");
        return _groundPickMask;
    }
}
