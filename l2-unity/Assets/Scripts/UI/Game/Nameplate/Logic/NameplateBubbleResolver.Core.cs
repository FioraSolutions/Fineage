using UnityEngine;

public sealed partial class NameplateBubbleResolver
{
    public L2TargetRenderType ResolveBubbleType(Transform target)
    {
        if (target == null)
        {
            return L2TargetRenderType.None;
        }

        bool hovering = IsHover(target);
        TargetManager targets = TargetPacketServices.Targets;
        if (targets != null &&
            targets.HasTarget() &&
            targets.Target.Data != null &&
            SameActor(targets.Target.Data.ObjectTransform, target))
        {
            // Sword while this mob is the attack target, and also while the cursor
            // is on an attackable that is already selected. The blue mark stays
            // when the pointer is elsewhere.
            if (targets.IsAttackTargetSet() || (hovering && Attackable(target)))
                return L2TargetRenderType.Attack;

            return L2TargetRenderType.Target;
        }

        if (hovering)
            return L2TargetRenderType.Normal;

        return L2TargetRenderType.None;
    }
}
