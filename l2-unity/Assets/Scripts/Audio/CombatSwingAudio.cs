using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Local player melee swing/shing from weapongrp item_sound when attack animation starts.
/// </summary>
public static class CombatSwingAudio
{
    static readonly HashSet<int> LoggedMissingGear = new HashSet<int>();

    public static void PlayLocalPlayerWeaponSwing(PlayerEntity player)
    {
        if (player == null)
        {
            FineageLog.Error(FineageLogCategory.World, "CombatSwingAudio: local player null for WeaponSwing");
            return;
        }

        int entityId = player.Identity != null ? player.Identity.Id : 0;
        Vector3 position = player.transform.position;
        PlayerGear gear = player.GetComponent<PlayerGear>();
        if (gear == null)
        {
            if (LoggedMissingGear.Add(entityId))
            {
                FineageLog.Error(
                    FineageLogCategory.World,
                    "CombatSwingAudio: PlayerGear missing entityId=" + entityId + " event=WeaponSwing");
            }

            return;
        }

        int weaponId = gear.EquippedRightHandWeapon != null ? gear.EquippedRightHandWeapon.Id : 0;
        string swing = CombatHitSoundResolver.ResolveWeaponSwingToken(weaponId);
        string shing = CombatHitSoundResolver.ResolveWeaponShingToken(weaponId);

        CharacterAnimationAudioHandler handler =
            player.GetComponentInChildren<CharacterAnimationAudioHandler>(true);
        if (handler != null)
        {
            handler.PlayItemSoundTokenAtRatio(swing, handler.SwishRatio);
            handler.PlayItemSoundTokenAtRatio(shing, handler.AtkRatio);
            return;
        }

        if (!CharacterCombatAudioDispatch.PlayItemSoundToken(swing, position, entityId))
        {
            FineageLog.Error(
                FineageLogCategory.World,
                "CombatSwingAudio: WeaponSwing unresolved entityId=" + entityId
                + " weaponItemId=" + weaponId + " token=\"" + swing + "\"");
        }

        if (!CharacterCombatAudioDispatch.PlayItemSoundToken(shing, position, entityId))
        {
            FineageLog.Error(
                FineageLogCategory.World,
                "CombatSwingAudio: WeaponShing unresolved entityId=" + entityId
                + " weaponItemId=" + weaponId + " token=\"" + shing + "\"");
        }
    }
}