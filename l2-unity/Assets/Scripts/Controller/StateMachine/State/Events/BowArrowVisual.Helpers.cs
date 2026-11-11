using UnityEngine;

public sealed partial class BowArrowVisual
{
    static bool IsBowShot(UserEntity user, string animName)
    {
        return IsBowAnimName(animName) || user.WeaponAnim == "bow";
    }

    static bool IsBowAnimName(string animName)
    {
        return !string.IsNullOrEmpty(animName) &&
               animName.IndexOf("bow", System.StringComparison.OrdinalIgnoreCase) >= 0;
    }

    static Transform ResolveAimTarget(Entity entity)
    {
        if (entity == null)
            return null;
        if (entity.AttackTarget != null)
            return entity.AttackTarget;
        if (entity.ActionSlot != null && entity.ActionSlot.Target != null)
            return entity.ActionSlot.Target.transform;
        return entity.Target;
    }
}
