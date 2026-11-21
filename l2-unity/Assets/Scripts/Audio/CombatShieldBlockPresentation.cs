using UnityEngine;

/// <summary>
/// Interlude shield block react: <c>shieldatk_&lt;weaponSuffix&gt;</c> (AnimTween) + GlobalEffect shield_composite.
/// </summary>
public static class CombatShieldBlockPresentation
{
    public const int ShieldBlockImpactEffectId = 1040;

    public static string ResolveAnimatorStateName(Entity defender)
    {
        if (defender == null)
            return "shieldatk_1HS";
        string suffix = defender.Gear != null && !string.IsNullOrEmpty(defender.Gear.WeaponAnim)
            ? defender.Gear.WeaponAnim
            : "1HS";
        return "shieldatk_" + suffix;
    }

    public static bool TryPlayBlockAnimation(Entity defender)
    {
        if (defender == null || defender.Identity == null)
            return false;

        string state = ResolveAnimatorStateName(defender);
        if (ControllerPacketServices.Animations != null)
        {
            ControllerPacketServices.Animations.PlayExactAnimatorState(defender.Identity.Id, state);
            return true;
        }

        NetworkAnimationController nac = defender.GetAnimatorController();
        if (nac == null)
        {
            FineageLog.Error(
                FineageLogCategory.World,
                "CombatShieldBlock: missing animator state=\"" + state + "\" entityId="
                + defender.Identity.Id);
            return false;
        }

        nac.CrossFadeInFixedTime(state, 0.08f);
        return true;
    }

    public static Vector3 ResolveShieldImpactPoint(Entity defender, Vector3 fallback)
    {
        if (defender == null)
            return fallback;

        Gear gear = defender.Gear;
        if (gear != null)
        {
            Transform shieldBone = gear.ShieldAttachPoint;
            if (shieldBone != null)
                return shieldBone.position;
        }

        return fallback;
    }
}