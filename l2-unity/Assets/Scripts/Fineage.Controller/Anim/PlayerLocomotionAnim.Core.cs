using System;
using System.Collections.Generic;

public sealed partial class PlayerLocomotionAnim : PlayerAnimStateMapBase
{
    public static string FamilyPrefix(PlayerLocomotionFamily family)
    {
        switch (family)
        {
            case PlayerLocomotionFamily.Wait: return "wait_";
            case PlayerLocomotionFamily.Walk: return "walk_";
            case PlayerLocomotionFamily.Run: return "run_";
            case PlayerLocomotionFamily.AtkWait: return "atkwait_";
            default: return "wait_";
        }
    }

    public static string ToSuffix(PlayerAnimWeapon weapon) => ToWeaponSuffix(weapon);

    public static bool TryFromSuffix(string weaponSuffix, out PlayerAnimWeapon weapon) =>
        TryParseWeaponSuffix(weaponSuffix, out weapon);
}
