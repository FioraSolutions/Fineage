using UnityEngine;

/// <summary>
/// Maps hover / target / melee-attack state to L2 TargetRenderType bubbles.
/// </summary>
public sealed class NameplateBubbleResolver
{
    public L2TargetRenderType ResolveBubbleType(Transform target)
    {
        if (target == null)
        {
            return L2TargetRenderType.None;
        }

        TargetManager targets = TargetPacketServices.Targets;
        if (targets != null &&
            targets.HasTarget() &&
            targets.Target.Data != null &&
            targets.Target.Data.ObjectTransform == target)
        {
            if (targets.IsAttackTargetSet())
            {
                return L2TargetRenderType.Attack;
            }

            return L2TargetRenderType.Target;
        }

        if (ControllerPacketServices.Clicks != null &&
            ControllerPacketServices.Clicks.HoverObjectData != null &&
            ControllerPacketServices.Clicks.HoverObjectData.ObjectTransform == target)
        {
            return L2TargetRenderType.Normal;
        }

        return L2TargetRenderType.None;
    }

    /// <summary>
    /// Local player: suppress grey hover bubble; keep Target/Attack (e.g. StatusWindow self-select).
    /// </summary>
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
}
