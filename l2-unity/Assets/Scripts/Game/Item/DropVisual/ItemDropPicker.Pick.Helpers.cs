using UnityEngine;

public sealed partial class ItemDropPicker
{
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
