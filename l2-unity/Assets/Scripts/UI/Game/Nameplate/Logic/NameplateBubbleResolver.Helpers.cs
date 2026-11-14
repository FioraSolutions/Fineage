using UnityEngine;

public sealed partial class NameplateBubbleResolver
{
    public L2TargetRenderType ResolveForPaint(Transform target, bool isLocalPlayer)
    {
        L2TargetRenderType type = ResolveBubbleType(target);
        if (isLocalPlayer && type == L2TargetRenderType.Normal)
        {
            return L2TargetRenderType.None;
        }

        return type;
    }

    public bool IsHoverOrTarget(Transform target)
    {
        if (target == null)
        {
            return false;
        }

        if (ControllerPacketServices.Clicks != null &&
            ControllerPacketServices.Clicks.HoverObjectData != null &&
            ControllerPacketServices.Clicks.HoverObjectData.ObjectTransform == target)
        {
            return true;
        }

        TargetManager targets = TargetPacketServices.Targets;
        if (targets != null &&
            targets.HasTarget() &&
            targets.Target.Data.ObjectTransform == target)
        {
            return true;
        }

        return false;
    }

    static bool IsHover(Transform target)
    {
        return ControllerPacketServices.Clicks != null &&
            ControllerPacketServices.Clicks.HoverObjectData != null &&
            SameActor(ControllerPacketServices.Clicks.HoverObjectData.ObjectTransform, target);
    }

    static bool SameActor(Transform a, Transform b)
    {
        if (a == null || b == null)
            return false;
        if (a == b)
            return true;
        Entity ea = a.GetComponentInParent<Entity>();
        Entity eb = b.GetComponentInParent<Entity>();
        return ea != null && ea == eb;
    }

    static bool Attackable(Transform target)
    {
        if (target == null)
            return false;
        Entity entity = target.GetComponentInParent<Entity>();
        return EntityInteraction.IsAttackable(entity);
    }
}
