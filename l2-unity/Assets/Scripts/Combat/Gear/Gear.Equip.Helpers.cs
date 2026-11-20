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
    private void ActivateGameObject(GameObject go, Weapongrp grp, WeaponAttachEquipMode mode, Transform[] refreshAllBone)
    {
        if (go != null)
        {
            SetType(mode, grp, refreshAllBone);
            go.SetActive(true);
        }
    }

    private void ActivateDualHand(
        GameObject leftGo,
        GameObject rightGo,
        Weapongrp grp,
        Transform[] refreshAllBone)
    {
        if (grp == null)
            return;
        WeaponAttachProfile profile = WeaponAttachProfileResolver.Resolve(grp, WeaponAttachEquipMode.BothHands);
        if (profile.Mounts.Count >= 2)
        {
            if (rightGo != null)
            {
                AbstractMeshManager.ApplyWeaponMount(rightGo, profile.Mounts[0], refreshAllBone);
                rightGo.SetActive(true);
            }
            if (leftGo != null)
            {
                AbstractMeshManager.ApplyWeaponMount(leftGo, profile.Mounts[1], refreshAllBone);
                leftGo.SetActive(true);
            }
            return;
        }

        if (rightGo != null && profile.Mounts.Count > 0)
        {
            AbstractMeshManager.ApplyWeaponMount(rightGo, profile.Mounts[0], refreshAllBone);
            rightGo.SetActive(true);
        }
    }

    private void ActivateArrow(GameObject go, string modelKey, Transform[] refreshAllBone)
    {
        if (go == null)
            return;
        var mount = new WeaponAttachMount(WeaponAttachSocket.RightHand, modelKey);
        AbstractMeshManager.ApplyWeaponMount(go, mount, refreshAllBone);
        go.SetActive(true);
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