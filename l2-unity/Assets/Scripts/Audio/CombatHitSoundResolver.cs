using UnityEngine;

/// <summary>
/// Interlude melee impact SFX from weapongrp item_sound + target defense_sound / armor, not catalog constants.
/// </summary>
public static class CombatHitSoundResolver
{
    public enum WeaponSoundIndex
    {
        Swing = 0,
        Shing = 1,
        ImpactPrimary = 2,
        ImpactSecondary = 3,
    }

    public static string ResolveFmodEventPath(
        bool criticalHit,
        Entity attacker,
        Entity target,
        int attackerWeaponItemId)
    {
        Weapongrp weaponGrp = null;
        if (attackerWeaponItemId > 0 && TablePacketServices.WeaponGrps != null)
            weaponGrp = TablePacketServices.WeaponGrps.GetWeapon(attackerWeaponItemId);

        Npcgrp npcGrp = null;
        if (target is NpcEntity npc && npc.NpcData != null)
            npcGrp = npc.NpcData.Npcgrp;

        return ResolveFmodEventPathFromData(criticalHit, weaponGrp, npcGrp, attackerWeaponItemId);
    }

    public static string ResolveFmodEventPathFromData(
        bool criticalHit,
        Weapongrp weaponGrp,
        Npcgrp targetNpc,
        int weaponItemIdForLog)
    {
        if (criticalHit)
            return AudioPathResolver.FmodSkillSound("critical_hit");

        string weaponToken = ResolveWeaponItemSound(weaponGrp, WeaponSoundIndex.ImpactPrimary, weaponItemIdForLog);
        if (string.IsNullOrEmpty(weaponToken))
            weaponToken = ResolveWeaponItemSound(weaponGrp, WeaponSoundIndex.ImpactSecondary, weaponItemIdForLog);
        if (string.IsNullOrEmpty(weaponToken))
            weaponToken = ResolveWeaponItemSound(weaponGrp, WeaponSoundIndex.Shing, weaponItemIdForLog);

        string defenseToken = ResolveNpcDefenseSound(targetNpc);
        string chosenToken = !string.IsNullOrEmpty(weaponToken) ? weaponToken : defenseToken;
        if (string.IsNullOrEmpty(chosenToken))
            chosenToken = "ItemSound.armor_underwear_1";

        if (!InterludeSoundTokenPath.TryFromDatToken(chosenToken, out string path))
        {
            FineageLog.Error(
                FineageLogCategory.World,
                "CombatHitSound: unresolved token=\"" + chosenToken + "\" weaponItemId="
                + weaponItemIdForLog);
            path = AudioPathResolver.FmodItemSound("armor_underwear_1");
        }

        if (InterludeSoundTokenPath.IsUiOrInterfaceEvent(path))
        {
            FineageLog.Error(
                FineageLogCategory.World,
                "CombatHitSound: refused UI/interface event path=\"" + path + "\" token=\"" + chosenToken
                + "\" weaponItemId=" + weaponItemIdForLog);
            path = AudioPathResolver.FmodItemSound("armor_underwear_1");
        }

        return path;
    }

    public static string ResolveWeaponSwingToken(int weaponItemId)
    {
        Weapongrp grp = weaponItemId > 0 && TablePacketServices.WeaponGrps != null
            ? TablePacketServices.WeaponGrps.GetWeapon(weaponItemId)
            : null;
        return ResolveWeaponItemSound(grp, WeaponSoundIndex.Swing, weaponItemId);
    }

    public static string ResolveWeaponShingToken(int weaponItemId)
    {
        Weapongrp grp = weaponItemId > 0 && TablePacketServices.WeaponGrps != null
            ? TablePacketServices.WeaponGrps.GetWeapon(weaponItemId)
            : null;
        return ResolveWeaponItemSound(grp, WeaponSoundIndex.Shing, weaponItemId);
    }

    static string ResolveWeaponItemSound(Weapongrp grp, WeaponSoundIndex index, int weaponItemIdForLog)
    {
        if (grp?.ItemSounds == null || grp.ItemSounds.Length == 0)
        {
            if (weaponItemIdForLog > 0)
            {
                FineageLog.Error(
                    FineageLogCategory.World,
                    "CombatHitSound: missing weapongrp item_sound weaponItemId=" + weaponItemIdForLog);
            }

            return null;
        }

        int i = (int)index;
        if (i < 0 || i >= grp.ItemSounds.Length)
            return null;

        string token = grp.ItemSounds[i];
        return string.IsNullOrWhiteSpace(token) ? null : token;
    }

    static string ResolveNpcDefenseSound(Npcgrp npcGrp)
    {
        if (npcGrp?.DefenseSounds == null || npcGrp.DefenseSounds.Length == 0)
            return null;

        int pick = UnityEngine.Random.Range(0, npcGrp.DefenseSounds.Length);
        return npcGrp.DefenseSounds[pick];
    }

    public static int ResolveAttackerWeaponItemId(Entity attacker)
    {
        if (attacker == null)
            return 0;

        if (attacker.Gear?.EquippedRightHandWeapon != null)
            return attacker.Gear.EquippedRightHandWeapon.Id;

        if (attacker.Appearance != null && attacker.Appearance.RHand > 0)
            return attacker.Appearance.RHand;

        return 0;
    }
}