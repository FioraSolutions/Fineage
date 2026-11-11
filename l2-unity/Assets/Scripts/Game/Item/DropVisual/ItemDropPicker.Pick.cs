using UnityEngine;

public sealed partial class ItemDropPicker
{
    static void AddLayer(ref int mask, string name)
    {
        int layer = LayerMask.NameToLayer(name);
        if (layer >= 0)
            mask |= 1 << layer;
    }

    static void Collect(
        int count,
        Ray ray,
        float maxDist,
        float maxRadial,
        bool hasGround,
        Vector3 groundPoint,
        ref ItemEntity best,
        ref float bestRadial,
        ref float bestAlong)
    {
        for (int i = 0; i < count; i++)
        {
            Collider col = OverlapBuffer[i];
            if (col == null)
                continue;
            ItemEntity candidate = col.GetComponentInParent<ItemEntity>();
            if (candidate == null)
                continue;

            float along = AlongRay(ray, col.bounds.center);
            if (along < 0f || along > maxDist)
                continue;

            Vector3 onRay = ray.origin + ray.direction * along;
            float radial = Vector3.Distance(onRay, col.ClosestPoint(onRay));
            float groundDist = hasGround
                ? Vector3.Distance(groundPoint, col.ClosestPoint(groundPoint))
                : float.MaxValue;
            float score = Mathf.Min(radial, groundDist);
            if (score > maxRadial)
                continue;

            if (score < bestRadial - 0.001f ||
                (Mathf.Abs(score - bestRadial) <= 0.001f && along < bestAlong))
            {
                best = candidate;
                bestRadial = score;
                bestAlong = along;
            }
        }
    }

    static bool TryItemHoverDistance(
        ItemEntity entity,
        Ray ray,
        float maxDist,
        bool hasGround,
        Vector3 groundPoint,
        out float hoverDist)
    {
        hoverDist = float.MaxValue;
        Collider[] colliders = entity.GetComponentsInChildren<Collider>();
        bool any = false;
        for (int i = 0; i < colliders.Length; i++)
        {
            Collider col = colliders[i];
            if (col == null || !col.enabled)
                continue;
            float along = AlongRay(ray, col.bounds.center);
            if (along < 0f || along > maxDist)
                continue;
            Vector3 onRay = ray.origin + ray.direction * along;
            hoverDist = Mathf.Min(hoverDist, Vector3.Distance(onRay, col.ClosestPoint(onRay)));
            if (hasGround)
                hoverDist = Mathf.Min(hoverDist, Vector3.Distance(groundPoint, col.ClosestPoint(groundPoint)));
            any = true;
        }

        return any;
    }

    static float AlongRay(Ray ray, Vector3 worldPoint)
    {
        return Vector3.Dot(worldPoint - ray.origin, ray.direction);
    }
}
