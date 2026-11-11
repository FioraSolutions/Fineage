using UnityEngine;

public static partial class EffectLightPlacement
{
    public static bool TryResolve(
        EffectResolveContext context,
        LightEffectSetting settings,
        out Vector3 lightPoint,
        out Vector3 lightDir)
    {
        lightPoint = Vector3.zero;
        lightDir = Vector3.forward;

        if (settings == null)
        {
            return false;
        }

        Vector3 aim = ResolveAimDirection(context);
        lightDir = aim;

        switch (settings.attachSubject)
        {
            case LightAttachSubject.Caster:
                lightPoint = ResolveEntityCenter(context != null ? context.CasterEntity : null,
                    context != null ? context.CasterTransform : null,
                    fallback: Vector3.zero);
                if (lightPoint.sqrMagnitude < 0.0001f && context != null && context.HasHitPoint)
                {
                    lightPoint = context.HitPoint;
                }

                if (settings.useFaceOffset)
                {
                    // Caster: nudge toward target along aim.
                    lightPoint += aim * 0.2f;
                }

                return true;

            case LightAttachSubject.Target:
            {
                Entity target = context != null ? context.TargetEntity : null;
                if (target == null)
                {
                    target = TryResolvePlayerTarget();
                }

                if (target != null)
                {
                    lightPoint = ResolveFaceOrCenter(target, aim, settings);
                    return true;
                }

                // Impact FX often has HitPointProxy owner — no TargetEntity.
                if (context != null && context.HasHitPoint)
                {
                    lightPoint = ApplyFaceOffsetToPoint(context.HitPoint, aim, settings, 0.35f);
                    return true;
                }

                return false;
            }

            case LightAttachSubject.HitPoint:
            default:
                if (context == null || !context.HasHitPoint)
                {
                    return false;
                }

                lightPoint = ApplyFaceOffsetToPoint(context.HitPoint, aim, settings, 0.35f);
                return true;
        }
    }

    private static Vector3 ResolveAimDirection(EffectResolveContext context)
    {
        if (context != null && context.HasHitDirection && context.HitDirection.sqrMagnitude > 0.0001f)
        {
            return FlatNormalized(context.HitDirection);
        }

        if (context != null &&
            context.CasterTransform != null &&
            context.TargetTransform != null)
        {
            Vector3 toTarget = FlatNormalized(context.TargetTransform.position - context.CasterTransform.position);
            if (toTarget.sqrMagnitude > 0.0001f)
            {
                return toTarget;
            }
        }

        if (context != null && context.HasHitPoint && DomainPacketServices.LocalPlayer != null)
        {
            Vector3 toHit = FlatNormalized(context.HitPoint - DomainPacketServices.LocalPlayer.transform.position);
            if (toHit.sqrMagnitude > 0.0001f)
            {
                return toHit;
            }
        }

        return Vector3.forward;
    }
}
