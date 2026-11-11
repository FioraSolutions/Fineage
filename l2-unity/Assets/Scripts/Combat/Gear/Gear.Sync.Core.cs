using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ModelTable;
using static Unity.Burst.Intrinsics.Arm;
using static UnityEngine.Rendering.DebugUI;

public partial class Gear
{
    public void SyncEquippedWeapons(int rHandId, int lHandId)
    {
        Weapon rWeapon = rHandId != 0 ? TablePacketServices.Items.GetWeapon(rHandId) : null;
        Weapon lItem = lHandId != 0 ? TablePacketServices.Items.GetWeapon(lHandId) : null;
        WeaponType rType = rWeapon != null && rWeapon.Weapongrp != null
            ? rWeapon.Weapongrp.WeaponType : WeaponType.none;
        WeaponType lType = lItem != null && lItem.Weapongrp != null
            ? lItem.Weapongrp.WeaponType : WeaponType.none;

        // dual swords + dual fists / castets (lrhand). Fist starter (Viper's Fang) is
        // WeaponType.fist from weapongrp handness=7 — still needs both hand meshes.
        bool dual = rType == WeaponType.dual
            || rType == WeaponType.dualfist
            || rType == WeaponType.fist;
        bool bow = rType == WeaponType.bow;
        bool lShield = !dual && (lType == WeaponType.shield || lType == WeaponType.none && lHandId != 0 && lItem != null);

        int wantRight = 0;
        int wantLeft = 0;
        int wantShield = 0;
        if (dual)
        {
            wantRight = rHandId;
            wantLeft = rHandId;
        }
        else if (bow)
        {
            wantLeft = rHandId;
        }
        else
        {
            wantRight = rHandId;
            if (lShield)
                wantShield = lHandId;
            else
                wantLeft = lHandId;
        }

        int haveRight = _rightHandWeapon != null ? _rightHandWeapon.Id : 0;
        int haveShield = _leftHandShield != null ? _leftHandShield.Id : 0;
        int haveLeft = 0;
        if (_leftHandWeapon != null && _leftHandShield == null)
            haveLeft = _leftHandWeapon.Id;

        if (haveRight == wantRight && haveLeft == wantLeft && haveShield == wantShield &&
            HasExpectedHandMeshes(wantRight, wantLeft, wantShield) &&
            !HasOrphanHandMeshes(wantRight, wantLeft, wantShield))
            return;

        ClearAllHandEquipment();

        if (dual)
            EquipLeftAndRightWeapon(rHandId);
        else if (bow)
            EquipWeapon(rHandId, false);
        else
        {
            if (wantRight != 0)
                EquipWeapon(wantRight, false);
            if (wantShield != 0)
                EquipShield(wantShield);
            else if (wantLeft != 0)
                EquipWeapon(wantLeft, true);
        }
    }

    static bool IsTwoHandedWeapon(Weapon weapon)
    {
        if (weapon?.Weapongrp == null)
            return false;
        if (weapon.Weapongrp.BodyPart == ItemSlot.lrhand)
            return true;
        WeaponType type = weapon.Weapongrp.WeaponType;
        return type == WeaponType.pole
            || type == WeaponType.staff
            || type == WeaponType.bigword
            || type == WeaponType.bigblunt
            || type == WeaponType.bow
            || type == WeaponType.dual
            || type == WeaponType.dualfist;
    }

    void ClearHandsForNewWeapon(bool leftSlot, bool dual, bool twoHand = false)
    {
        if (dual || twoHand)
        {
            ClearAllHandEquipment();
            return;
        }

        if (leftSlot)
            ClearLeftHandForShield();
        else
        {
            if (IsDualEquipped())
                UnequipWeapon(false, _rightHandWeapon.Id, true);
            else if (_rightHandWeapon != null)
                UnequipWeapon(false, _rightHandWeapon.Id, false);
            ClearWeaponMeshesOnBone(GetRightHandBone());
            RefreshData(false, null);
        }
    }

    void ClearLeftHandForShield()
    {
        if (_leftHandShield != null)
            UnequipShield(_leftHandShield.Id);
        else if (_leftHandWeapon != null)
            UnequipWeapon(true, _leftHandWeapon.Id, false);
        ClearWeaponMeshesOnBone(GetLeftHandBone());
        ClearShieldMeshesOnBone(GetShieldBone());
        RefreshData(true, null);
        RefreshDataShield(null);
    }

    void ClearAllHandEquipment()
    {
        if (IsDualEquipped())
            UnequipWeapon(false, _rightHandWeapon.Id, true);
        else
        {
            if (_leftHandShield != null)
                UnequipShield(_leftHandShield.Id);
            else if (_leftHandWeapon != null)
                UnequipWeapon(true, _leftHandWeapon.Id, false);
            if (_rightHandWeapon != null)
                UnequipWeapon(false, _rightHandWeapon.Id, false);
        }

        ClearWeaponMeshesOnBone(GetLeftHandBone());
        ClearWeaponMeshesOnBone(GetRightHandBone());
        ClearShieldMeshesOnBone(GetShieldBone());
        RefreshData(true, null);
        RefreshData(false, null);
        RefreshDataShield(null);
    }

    bool IsDualEquipped()
    {
        return _rightHandWeapon != null && _leftHandWeapon != null &&
            _leftHandShield == null &&
            _rightHandWeapon.Id == _leftHandWeapon.Id;
    }
}
