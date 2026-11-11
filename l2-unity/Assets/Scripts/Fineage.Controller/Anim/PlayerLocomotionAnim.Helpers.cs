using System;
using System.Collections.Generic;

public sealed partial class PlayerLocomotionAnim : PlayerAnimStateMapBase
{
    public static string ToStateName(PlayerLocomotionFamily family, PlayerAnimWeapon weapon) =>
        ComposeStateName(FamilyPrefix(family), weapon);

    public static bool TryParseFamilyPrefix(string animName, out PlayerLocomotionFamily family, out string suffix)
    {
        family = default;
        suffix = null;
        if (!Instance.TryResolveStateName(animName, out _, out string matchedPrefix))
        {
            return false;
        }

        if (!TryFamilyFromPrefix(matchedPrefix, out family))
        {
            return false;
        }

        suffix = animName.Substring(matchedPrefix.Length);
        return true;
    }

    public static bool TryResolve(string animName, out string stateName, out PlayerLocomotionFamily family)
    {
        stateName = null;
        family = default;
        if (!Instance.TryResolveStateName(animName, out stateName, out string matchedPrefix))
        {
            return false;
        }

        return TryFamilyFromPrefix(matchedPrefix, out family);
    }

    static bool TryFamilyFromPrefix(string prefix, out PlayerLocomotionFamily family)
    {
        switch (prefix)
        {
            case "atkwait_":
                family = PlayerLocomotionFamily.AtkWait;
                return true;
            case "wait_":
                family = PlayerLocomotionFamily.Wait;
                return true;
            case "walk_":
                family = PlayerLocomotionFamily.Walk;
                return true;
            case "run_":
                family = PlayerLocomotionFamily.Run;
                return true;
            default:
                family = default;
                return false;
        }
    }
}
