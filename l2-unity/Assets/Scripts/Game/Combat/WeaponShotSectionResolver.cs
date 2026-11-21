/// <summary>Interlude soul/spirit shot INI section keys from equipped gear (boundary resolver).</summary>
public static class WeaponShotSectionResolver
{
    public static string Resolve(Gear gear)
    {
        if (gear == null)
            return "HAND";

        CombatStyleProfile style = gear.CombatStyle;
        if (style.IsResolved && style.IsRanged)
            return "BOW";
        if (style.IsResolved && style.IsDualAnimSuffix)
            return "DUAL";

        WeaponType right = gear.RightHandWeaponType;
        WeaponType left = gear.LeftHandWeaponType;
        if (left == WeaponType.shield || left == WeaponType.arrow)
            left = WeaponType.none;

        if (right == WeaponType.mbook || left == WeaponType.mbook)
            return "BOOK";
        if (right == WeaponType.dagger || left == WeaponType.dagger)
            return "THROW";
        if (right == WeaponType.fist || left == WeaponType.fist ||
            right == WeaponType.dualfist || left == WeaponType.dualfist)
            return "DUALFIST";
        if (IsBare(right) && IsBare(left))
            return "HAND";
        if (right == WeaponType.bigword || right == WeaponType.bigblunt ||
            left == WeaponType.bigword || left == WeaponType.bigblunt)
            return "2HS";
        if (right == WeaponType.pole || right == WeaponType.staff ||
            left == WeaponType.pole || left == WeaponType.staff)
            return "POLE";

        if (!style.IsResolved)
        {
            FineageLog.Error(
                FineageLogCategory.World,
                "WeaponShotSection: unresolved CombatStyleProfile; defaulting to 1HS. right=" + right +
                " left=" + left);
        }

        return "1HS";
    }

    static bool IsBare(WeaponType type) =>
        type == WeaponType.none || type == WeaponType.hand;
}