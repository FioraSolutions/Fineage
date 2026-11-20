using System.Collections.Generic;

/// <summary>
/// Builds <see cref="CombatStyleProfile"/> from weapongrp / weapon type at the equip boundary only.
/// </summary>
public static class CombatStyleProfileResolver
{
    static readonly HashSet<int> LoggedMissingGrp = new HashSet<int>();

    public static CombatStyleProfile Resolve(Weapongrp grp, int weaponItemId)
    {
        if (grp == null)
        {
            LogMissingWeapongrp(weaponItemId);
            return CombatStyleProfile.Unresolved;
        }

        return ResolveInternal(grp.WeaponType, weaponItemId);
    }

    public static CombatStyleProfile ResolveBareHand() => CombatStyleProfile.BareHand;

    public static CombatStyleProfile ResolveForWeaponType(WeaponType weaponType, int weaponItemId = 0)
    {
        if (weaponType == WeaponType.none)
            return CombatStyleProfile.BareHand;

        return ResolveInternal(weaponType, weaponItemId);
    }

    static CombatStyleProfile ResolveInternal(WeaponType weaponType, int weaponItemId)
    {
        string suffix = WeaponTypeParser.GetWeaponAnim(weaponType);
        if (string.IsNullOrEmpty(suffix))
        {
            FineageLog.Error(
                FineageLogCategory.World,
                "CombatStyleProfile: empty AnimSuffix for weaponItemId=" + weaponItemId +
                " weaponType=" + weaponType);
            return CombatStyleProfile.Unresolved;
        }

        int range = WeaponTypeParser.WeaponRange(weaponType);
        bool isRanged = weaponType == WeaponType.bow;
        bool isDual = suffix == "dual";
        float hitFraction = isRanged
            ? CombatStyleProfile.InterludeBowHitFraction
            : CombatStyleProfile.InterludeMeleeHitFraction;

        return new CombatStyleProfile(
            suffix,
            range,
            isRanged,
            isDual,
            hitFraction,
            isResolved: true);
    }

    public static void LogMissingWeapongrp(int weaponItemId)
    {
        if (weaponItemId <= 0)
        {
            FineageLog.Error(
                FineageLogCategory.World,
                "CombatStyleProfile: missing Weapongrp (weaponItemId=" + weaponItemId + ")");
            return;
        }

        lock (LoggedMissingGrp)
        {
            if (!LoggedMissingGrp.Add(weaponItemId))
                return;
        }

        FineageLog.Error(
            FineageLogCategory.World,
            "CombatStyleProfile: missing Weapongrp for weaponItemId=" + weaponItemId +
            " — check Weapongrp table / item id");
    }
}