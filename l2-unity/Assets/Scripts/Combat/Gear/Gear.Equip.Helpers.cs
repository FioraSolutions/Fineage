using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ModelTable;
using static Unity.Burst.Intrinsics.Arm;
using static UnityEngine.Rendering.DebugUI;
using System;

public partial class Gear
{
    private void ActivateGameObject(GameObject go, WeaponType type , bool leftSlot , Transform[] refreshAllBone)
    {
        if (go != null)
        {
            SetType(type, leftSlot, refreshAllBone);
            go.SetActive(true);
        }
    }

    private void ActivateOtherGameObject(GameObject go, WeaponType type, bool leftSlot, Transform[] refreshAllBone)
    {
        if (go != null)
        {
            SetTypeOtherGo(go , type, leftSlot, refreshAllBone);
            go.SetActive(true);
        }
    }

    private void ErrorPrint(Weapon weapon , int weaponId)
    {
        if (weapon == null)
        {
            // Debug.LogWarning("Gear->EquipWeapon: Not Found item in database id " + weaponId);
        }

    }

    private void ApplyCombatStyleProfile(CombatStyleProfile profile)
    {
        _lastWeaponAnim = _weaponAnim;
        _combatStyle = profile ?? CombatStyleProfile.Unresolved;
        _weaponAnim = _combatStyle.AnimSuffix;
        _weaponRange = _combatStyle.AttackRangeUu;
    }

    private void UpdateWeaponType(WeaponType weaponType)
    {
        ApplyCombatStyleProfile(CombatStyleProfileResolver.ResolveForWeaponType(weaponType));
    }

    private void UpdateCombatStyleFromWeapon(Weapon weapon)
    {
        int itemId = weapon != null ? weapon.Id : 0;
        Weapongrp grp = weapon?.Weapongrp;
        ApplyCombatStyleProfile(CombatStyleProfileResolver.Resolve(grp, itemId));
    }
}
