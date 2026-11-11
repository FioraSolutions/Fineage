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

    private void UpdateWeaponType(WeaponType weaponType) {
        _lastWeaponAnim = _weaponAnim;
        _weaponAnim = WeaponTypeParser.GetWeaponAnim(weaponType);
        _weaponRange = WeaponTypeParser.WeaponRange(weaponType);
    }
}
