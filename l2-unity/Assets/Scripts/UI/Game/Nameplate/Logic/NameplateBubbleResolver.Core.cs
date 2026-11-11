using UnityEngine;

public sealed partial class NameplateBubbleResolver
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
}
