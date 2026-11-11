using UnityEngine;

public static partial class L2NameplateAnchor
{
    public static float CollisionHeightToUnityMeters(float collisionHeight)
    {
        if (collisionHeight <= 0.0001f)
            return DefaultCollisionHeightMeters;

        // Raw Interlude UU half-heights are typically ~3..40; converted meters are usually under ~1.
        if (collisionHeight > 2.5f)
            return collisionHeight / 52.5f;

        return collisionHeight;
    }

    public static float HeightFromFeet(float collisionHeight, float headHeightOffset)
    {
        float ch = CollisionHeightToUnityMeters(collisionHeight);
        return 2f * ch + headHeightOffset;
    }
}
