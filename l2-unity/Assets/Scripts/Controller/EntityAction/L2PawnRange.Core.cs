using System;
using UnityEngine;

public sealed partial class L2PawnRange
{
    public float AdjustSkipMeters()
    {
        return VectorUtils.ConvertL2UuToMeters(AdjustSkipUu);
    }

    public bool IsBow(Entity entity)
    {
        UserEntity user = entity as UserEntity;
        if (user != null)
            return string.Equals(user.WeaponAnim, "bow", StringComparison.OrdinalIgnoreCase);

        return entity != null && entity.Gear != null &&
               !string.IsNullOrEmpty(entity.Gear.WeaponAnim) &&
               string.Equals(entity.Gear.WeaponAnim, "bow", StringComparison.OrdinalIgnoreCase);
    }

    public Vector3 StopPointOnDistRing(Vector3 origin, Vector3 pawnLocation, float distMeters)
    {
        Vector3 fromPawn = VectorUtils.To2D(origin) - VectorUtils.To2D(pawnLocation);
        fromPawn.y = 0f;
        float mag = fromPawn.magnitude;
        if (mag < 0.001f || distMeters <= 0.01f)
            return origin;

        Vector3 dir = fromPawn / mag;
        return new Vector3(pawnLocation.x, origin.y, pawnLocation.z) + dir * distMeters;
    }

    public bool ShouldSkipCharInfoPositionSnap(Entity entity, Vector3 newPos)
    {
        if (!(entity is UserEntity))
            return false;

        float d = VectorUtils.Distance2D(entity.transform.position, newPos);
        if (d <= 0.15f)
            return true;

        if (HasReachedPawnDist(entity) && d < AdjustSkipMeters())
            return true;

        return IsBow(entity) && d < AdjustSkipMeters();
    }
}
