using UnityEngine;

/// <summary>
/// Shared soulshot/spiritshot consume and recharge presentation (AttackShot + MagicSkillUse).
/// </summary>
public static class WeaponChargePresentation
{
    public static void PlayMeleeSoulshotConsume(Entity entity)
    {
        if (entity == null)
            return;

        int skillId = ResolveEquippedSoulshotSkill(entity);
        if (skillId <= 0)
        {
            Debug.LogError(
                "[WeaponCharge] melee soulshot consume without charge skill — check weapon grade / weapongrp.");
            return;
        }

        WeaponChargeSkillPlayback.PlayChargeImpactSound(
            skillId,
            entity.transform.position);
    }

    static int ResolveEquippedSoulshotSkill(Entity entity)
    {
        PlayerEntity player = entity as PlayerEntity;
        if (player == null || player.Gear == null)
            return WeaponChargeSkill.SoulshotSkillIdForGrade(ItemGrade.none);

        Weapon weapon = player.Gear.EquippedRightHandWeapon;
        if (weapon?.Weapongrp == null)
            return WeaponChargeSkill.SoulshotSkillIdForGrade(ItemGrade.none);

        return WeaponChargeSkill.SoulshotSkillIdForGrade(weapon.Weapongrp.Grade);
    }
}