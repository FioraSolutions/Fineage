/// <summary>Read cached combat style from entities without weapon-type branching.</summary>
public static class CombatStyleEntity
{
    public static CombatStyleProfile ResolveFromItemId(int weaponItemId)
    {
        if (weaponItemId <= 0)
            return CombatStyleProfile.BareHand;

        if (TablePacketServices.WeaponGrps == null)
        {
            CombatStyleProfileResolver.LogMissingWeapongrp(weaponItemId);
            return CombatStyleProfile.Unresolved;
        }

        Weapongrp grp = TablePacketServices.WeaponGrps.GetWeapon(weaponItemId);
        return CombatStyleProfileResolver.Resolve(grp, weaponItemId);
    }

    public static CombatStyleProfile From(Entity entity)
    {
        if (entity == null)
            return CombatStyleProfile.Unresolved;

        if (entity.Gear != null)
            return entity.Gear.CombatStyle;

        return CombatStyleProfile.Unresolved;
    }

    public static bool IsRanged(Entity entity) => From(entity).IsRanged;
}