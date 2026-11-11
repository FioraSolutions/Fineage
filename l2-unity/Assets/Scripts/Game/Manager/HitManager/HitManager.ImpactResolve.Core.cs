using UnityEngine;
using System;
using System.Collections.Generic;
using VContainer;

public partial class HitManager
{
    private static Vector3 ResolveSoulshotImpactDirection(Entity attacker, Entity target, Vector3 hitPointFallback)
    {
        Transform attackerTf = ResolveAttackerTransform(attacker);
        Transform targetTf = target != null ? target.transform : null;

        if (attackerTf != null && targetTf != null)
        {
            Vector3 toTarget = Flat(targetTf.position - attackerTf.position);
            if (toTarget.sqrMagnitude > 0.0001f)
            {
                return toTarget;
            }
        }

        if (attackerTf != null)
        {
            Vector3 toHit = Flat(hitPointFallback - attackerTf.position);
            if (toHit.sqrMagnitude > 0.0001f)
            {
                return toHit;
            }
        }

        return Vector3.forward;
    }

    private static Vector3 ResolveSoulshotSpawnPoint(Entity target, Vector3 impactDir, Vector3 hitPointFallback)
    {
        if (target == null)
        {
            return hitPointFallback;
        }

        Vector3 center = ResolveTargetImpactCenter(target);
        float radius = ResolveTargetCollisionRadius(target) * SoulshotSpawnRadiusScale;
        Vector3 dir = Flat(impactDir);
        if (dir.sqrMagnitude < 0.0001f || radius <= 0.0001f)
        {
            return center;
        }

        // L2: Location - Transform((CollisionRadius*k, 0, 0), Rotation) → toward attacker.
        return center - dir * radius;
    }

    private static float ResolveTargetCollisionRadius(Entity target)
    {
        if (target == null)
        {
            return 0.35f;
        }

        // Prefer CharacterController — already Unity meters (dog vs humanoid).
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

        // Appearance.CollisionRadius is inconsistent across spawn paths:
        // NpcgrpTable already divides by 52.5 (Unity meters); some packets store L2 UU.
        if (target.Appearance != null && target.Appearance.CollisionRadius > 0.0001f)
        {
            float raw = target.Appearance.CollisionRadius;
            // L2 UU radii are typically >= ~5; Unity meters after /52.5 are usually < ~2.
            float fromAppearance = raw > 3f
                ? VectorUtils.ConvertL2jDistance(raw)
                : raw;
            if (fromAppearance > 0.01f)
            {
                return fromAppearance;
            }
        }

        if (TryHorizontalBoundsExtent(target, out float extent) && extent > 0.01f)
        {
            return extent;
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
}
