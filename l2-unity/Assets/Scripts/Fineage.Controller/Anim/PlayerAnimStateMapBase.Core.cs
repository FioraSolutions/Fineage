using System;
using System.Collections.Generic;

public abstract partial class PlayerAnimStateMapBase
{
    public static string ToWeaponSuffix(PlayerAnimWeapon weapon)
    {
        switch (weapon)
        {
            case PlayerAnimWeapon.Hand: return "hand";
            case PlayerAnimWeapon.OneHS: return "1HS";
            case PlayerAnimWeapon.TwoHS: return "2HS";
            case PlayerAnimWeapon.Pole: return "pole";
            case PlayerAnimWeapon.Dual: return "dual";
            case PlayerAnimWeapon.Bow: return "bow";
            default: return "hand";
        }
    }

    public static bool TryParseWeaponSuffix(string weaponSuffix, out PlayerAnimWeapon weapon)
    {
        weapon = PlayerAnimWeapon.Hand;
        if (string.IsNullOrEmpty(weaponSuffix))
        {
            return false;
        }

        switch (weaponSuffix)
        {
            case "hand":
                weapon = PlayerAnimWeapon.Hand;
                return true;
            case "1HS":
                weapon = PlayerAnimWeapon.OneHS;
                return true;
            case "2HS":
                weapon = PlayerAnimWeapon.TwoHS;
                return true;
            case "pole":
                weapon = PlayerAnimWeapon.Pole;
                return true;
            case "dual":
                weapon = PlayerAnimWeapon.Dual;
                return true;
            case "bow":
                weapon = PlayerAnimWeapon.Bow;
                return true;
            default:
                return false;
        }
    }
}
