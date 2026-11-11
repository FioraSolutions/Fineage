using UnityEngine;
using System;
using System.Collections.Generic;
using VContainer;

public partial class HitManager
{
    private static bool TryHorizontalBoundsExtent(Entity target, out float extent)
    {
        extent = 0f;
        if (target == null)
        {
            return false;
        }

        Renderer[] renderers = target.GetComponentsInChildren<Renderer>(true);
        if (renderers == null || renderers.Length == 0)
        {
            return false;
        }

        bool hasBounds = false;
        Bounds bounds = default;
        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer renderer = renderers[i];
            if (renderer == null || renderer.GetComponentInParent<BaseEffect>() != null)
            {
                continue;
            }

            if (!hasBounds)
            {
                bounds = renderer.bounds;
                hasBounds = true;
            }
            else
            {
                bounds.Encapsulate(renderer.bounds);
            }
        }

        if (!hasBounds)
        {
            return false;
        }

        extent = Mathf.Max(bounds.extents.x, bounds.extents.z);
        return extent > 0.01f;
    }

    private static Vector3 ResolveTargetImpactCenter(Entity target)
    {
        if (target == null)
        {
            return Vector3.zero;
        }

        CharacterController controller = ResolveCharacterController(target);
        if (controller != null)
        {
            return controller.transform.TransformPoint(controller.center);
        }

        return target.transform.position;
    }

    private static Transform ResolveAttackerTransform(Entity attacker)
    {
        if (attacker != null && attacker.transform != null)
        {
            return attacker.transform;
        }

        return DomainPacketServices.LocalPlayer != null ? DomainPacketServices.LocalPlayer.transform : null;
    }

    private static Entity ResolveTargetEntity(Entity targetEntity)
    {
        return targetEntity;
    }
}
