using UnityEngine;

public static partial class EffectLightPlacement
{
    private static Vector3 ResolveFaceOrCenter(Entity entity, Vector3 aim, LightEffectSetting settings)
    {
        Vector3 center = ResolveEntityCenter(entity, entity != null ? entity.transform : null, Vector3.zero);
        if (!settings.useFaceOffset)
        {
            return center;
        }

        float radius = ResolveCollisionRadius(entity);
        return center - aim * (radius * Mathf.Max(0.01f, settings.faceOffsetRadiusScale));
    }

    private static Vector3 ApplyFaceOffsetToPoint(
        Vector3 point,
        Vector3 aim,
        LightEffectSetting settings,
        float fallbackRadius)
    {
        if (!settings.useFaceOffset)
        {
            return point;
        }

        return point - aim * (fallbackRadius * Mathf.Max(0.01f, settings.faceOffsetRadiusScale));
    }
}
