using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public partial class EffectManager
{
    private static Vector3 hitPointFlat(Vector3 v)
    {
        v.y = 0f;
        return v.sqrMagnitude > 0.0001f ? v.normalized : Vector3.zero;
    }

    private static float AngleFlatDeg(Vector3 a, Vector3 b)
    {
        Vector3 af = hitPointFlat(a);
        Vector3 bf = hitPointFlat(b);
        if (af.sqrMagnitude < 0.0001f || bf.sqrMagnitude < 0.0001f)
        {
            return -1f;
        }

        return Vector3.Angle(af, bf);
    }

    private static Vector3 ResolveImpactDirection(Vector3 hitPoint, Vector3 impactDirection)
    {
        // Always flatten: sword hitDirection carries pitch and tilts the emitter.
        if (impactDirection.sqrMagnitude > 0.0001f)
        {
            Vector3 flat = impactDirection;
            flat.y = 0f;
            if (flat.sqrMagnitude > 0.0001f)
            {
                return flat.normalized;
            }
        }

        // L2: targetLoc - attackerLoc (horizontal).
        if (DomainPacketServices.LocalPlayer != null)
        {
            Vector3 fromAttacker = hitPoint - DomainPacketServices.LocalPlayer.transform.position;
            fromAttacker.y = 0f;
            if (fromAttacker.sqrMagnitude > 0.0001f)
            {
                return fromAttacker.normalized;
            }
        }

        return Vector3.forward;
    }
}
