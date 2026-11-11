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

    private static CharacterController ResolveCharacterController(Entity target)
    {
        if (target == null)
        {
            return null;
        }

        if (target is MonsterEntity monster)
        {
            CharacterController fromMonster = monster.GetCharacterController();
            if (fromMonster != null)
            {
                return fromMonster;
            }
        }
        else if (target is NpcEntity npc)
        {
            CharacterController fromNpc = npc.GetCharacterController();
            if (fromNpc != null)
            {
                return fromNpc;
            }
        }

        CharacterController controller = target.GetComponent<CharacterController>();
        if (controller == null)
        {
            controller = target.GetComponentInChildren<CharacterController>(true);
        }

        return controller;
    }

    private static Entity TryResolvePlayerTarget()
    {
        World world = WorldApplyPacketServices.GameWorld;
        if (DomainPacketServices.LocalPlayer == null || DomainPacketServices.LocalPlayer.TargetId <= 0 || world == null)
        {
            return null;
        }

        return world.GetEntityNoLockSync(DomainPacketServices.LocalPlayer.TargetId);
    }

    private static Vector3 FlatNormalized(Vector3 v)
    {
        v.y = 0f;
        return v.sqrMagnitude > 0.0001f ? v.normalized : Vector3.zero;
    }
}
