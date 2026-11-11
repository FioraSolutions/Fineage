using UnityEngine;

public static partial class EffectLightPlacement
{
    private static Vector3 ResolveEntityCenter(Entity entity, Transform transform, Vector3 fallback)
    {
        if (entity != null)
        {
            CharacterController controller = ResolveCharacterController(entity);
            if (controller != null)
            {
                return controller.transform.TransformPoint(controller.center);
            }

            if (entity.transform != null)
            {
                return entity.transform.position;
            }
        }

        if (transform != null)
        {
            return transform.position;
        }

        return fallback;
    }

    private static float ResolveCollisionRadius(Entity target)
    {
        if (target == null)
        {
            return 0.35f;
        }

        CharacterController controller = ResolveCharacterController(target);
        if (controller != null)
        {
            float scaleX = Mathf.Abs(controller.transform.lossyScale.x);
            float fromCapsule = controller.radius * scaleX;
            if (fromCapsule > 0.01f)
            {
                return fromCapsule;
            }
        }

        if (target.Appearance != null && target.Appearance.CollisionRadius > 0.0001f)
        {
            float raw = target.Appearance.CollisionRadius;
            float fromAppearance = raw > 3f ? VectorUtils.ConvertL2jDistance(raw) : raw;
            if (fromAppearance > 0.01f)
            {
                return fromAppearance;
            }
        }

        return 0.35f;
    }
}
