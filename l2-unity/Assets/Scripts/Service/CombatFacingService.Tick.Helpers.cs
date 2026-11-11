using System.Collections.Generic;
using UnityEngine;
using VContainer;
using System;

public sealed partial class CombatFacingService
{
    public static bool IsUsingBow(Entity entity)
    {
        if (entity is PlayerEntity player)
            return IsPlayerUsingBow(player);

        if (entity != null && entity.Appearance != null && Weapons != null)
        {
            if (IsBowItemId(entity.Appearance.RHand) || IsBowItemId(entity.Appearance.LHand))
                return true;
        }

        string anim = null;
        UserEntity user = entity as UserEntity;
        if (user != null)
            anim = user.WeaponAnim;
        else if (entity != null && entity.Gear != null)
            anim = entity.Gear.WeaponAnim;

        return !string.IsNullOrEmpty(anim) &&
               anim.IndexOf("bow", System.StringComparison.OrdinalIgnoreCase) >= 0;
    }

    public static bool IsPlayerUsingBow(PlayerEntity player)
    {
        if (player == null)
        {
            return false;
        }

        if (player.Appearance != null && Weapons != null)
        {
            if (IsBowItemId(player.Appearance.RHand) || IsBowItemId(player.Appearance.LHand))
            {
                return true;
            }
        }

        string anim = player.GetCurrentAnimName();
        return !string.IsNullOrEmpty(anim) &&
               anim.IndexOf("bow", System.StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private static bool IsBowItemId(int itemId)
    {
        if (itemId == 0)
        {
            return false;
        }

        Weapongrp weapon = Weapons.GetWeapon(itemId);
        return weapon != null && weapon.WeaponType == WeaponType.bow;
    }
}
