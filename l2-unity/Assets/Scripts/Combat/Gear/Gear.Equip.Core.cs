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
    public void EquipShield(int weaponId)
    {
        Weapon weapon = TablePacketServices.Items.GetWeapon(weaponId);
        ErrorPrint(weapon, weaponId);

        if (weaponId != 0 && weapon == null)
            EquipmentMeshLog.Once("weapon", weaponId);

        if (weaponId == 0 | IsShieldEquipped(weaponId) | weapon == null)
        {
            return;
        }

        ClearLeftHandForShield();

        GameObject weaponPrefab = (GameObject)LoadMesh(EquipmentCategory.Weapon, weaponId);
        if (weaponPrefab == null)
        {
            EquipmentMeshLog.Once("weapon", weaponId);
            return;
        }
        _origShieldPrefabName = weaponPrefab.name;
        // Shield alone must not force 1HS — bare-hand / shield-only use hand anim suffix.
        // Keep the right-hand weapon anim when a weapon is already equipped.
        WeaponType type = WeaponType.none;
        RefreshDataShield(weapon , WeaponType.shield);
        if (_rightHandWeapon != null)
            UpdateWeaponType(_rightHandType);
        else
            UpdateWeaponType(WeaponType.hand);
        string shieldNameId = shieldName+weaponId;
        Transform[] refreshAllBone = RefreshBone(allBone);
        GameObject go = CreateCopy(weaponPrefab, shieldNameId, ObjectType.Weapon);
        _lastWeaponAnim = _weaponAnim;
        ActivateGameObject(go, type, true, refreshAllBone);

        if (_rightHandType != _lastRightHandType | type != _lastShieldHandType 
            | _rightHandType == WeaponType.pole 
            | _rightHandType == WeaponType.staff)
        {
            OnEquipAnimationRefresh?.Invoke(-1, weapon);
        }
        else
        {
            //if we have nothing in our right hand
            if (_rightHandWeapon == null) OnEquipAnimationRefresh?.Invoke(-1, weapon);
        }

        _lastShieldHandType = type;
    }

    public virtual void EquipWeapon(int weaponId, bool leftSlot) {

        Weapon weapon = TablePacketServices.Items.GetWeapon(weaponId);
        leftSlot = weapon?.Weapongrp?.WeaponType == WeaponType.bow;


        ErrorPrint(weapon, weaponId);

        if (weaponId == 0 || weapon == null)
        {
            if (weaponId != 0)
                EquipmentMeshLog.Once("weapon", weaponId);
            return;
        }

        if (IsWeaponEquipped(weaponId, leftSlot))
            return;

        bool twoHand = IsTwoHandedWeapon(weapon);
        ClearHandsForNewWeapon(leftSlot, false, twoHand);

        GameObject weaponPrefab = (GameObject)LoadMesh(EquipmentCategory.Weapon, weaponId);
        if (weaponPrefab == null)
        {
            EquipmentMeshLog.Once("weapon", weaponId);
            return;
        }

        var weaponNameAndId = weaponName + weaponId;
        WeaponType type = weapon.Weapongrp.WeaponType;
        RefreshData(leftSlot, weapon);
        UpdateWeaponType(type);

        Transform[] refreshAllBone = RefreshBone(allBone);
        GameObject go = CreateCopy(weaponPrefab, weaponNameAndId, ObjectType.Weapon);


        ActivateGameObject(go, type, leftSlot, refreshAllBone);

        if(type != _lastRightHandType & _leftHandShield == null 
            | _rightHandType == WeaponType.pole
            | _rightHandType == WeaponType.staff
            | _rightHandType == WeaponType.bigword
            | _leftHandType == WeaponType.bow)
        {
            OnEquipAnimationRefresh?.Invoke(-1, weapon);
        }
        else
        {
            if (_leftHandWeapon == null | _leftHandShield == null) OnEquipAnimationRefresh?.Invoke(-1, weapon);
        }

        _lastRightHandType = type;

    }

    public void EquipArrowEtcItem(int etcId, bool leftSlot)
    {
        EtcItem etcItem = TablePacketServices.Items.GetEtcItem(etcId);

        if (etcId == 0 | IsWeaponEquipped(etcId, leftSlot) | etcItem == null)
        {
            return;
        }

        GameObject etcItemPrefab = (GameObject)LoadMesh(EquipmentCategory.EtcItem, etcId);
        if (etcItemPrefab == null)
        {
            EquipmentMeshLog.Once("item", etcId);
            return;
        }

        var etcIdNameAndId = etcName + etcId;
        Transform[] refreshAllBone = RefreshBone(allBone);
        GameObject go = CreateCopy(etcItemPrefab, etcIdNameAndId, ObjectType.Arrow);

        _goCurrentEtcItem = go;

        ActivateGameObject(go, WeaponType.arrow, leftSlot, refreshAllBone);

    }

    public void EquipLeftAndRightWeapon(int weaponId)
    {
        Weapon weapon = TablePacketServices.Items.GetWeapon(weaponId);

        ErrorPrint(weapon, weaponId);

        if (weaponId == 0 | weapon == null)
        {
            if (weaponId != 0)
                EquipmentMeshLog.Once("weapon", weaponId);
            return;
        }

        if (IsWeaponEquippedInBothHands(weaponId))
        {
            return;
        }

        ClearHandsForNewWeapon(false, true);

        GameObject weaponPrefab = (GameObject)LoadMesh(EquipmentCategory.Weapon, weaponId);
        if (weaponPrefab == null)
        {
            EquipmentMeshLog.Once("weapon", weaponId);
            return;
        }
        //GameObject weaponPrefabRight = (GameObject)LoadMesh(EquipmentCategory.Weapon, weaponId);

        //_origWeaponPrefabName = weaponPrefab.name;
        WeaponType type = weapon.Weapongrp.WeaponType;
        var weaponNameAndId = weaponName + weaponId;
        RefreshData(false, weapon);
        RefreshData(true, weapon);
        UpdateWeaponType(type);

        Transform[] refreshAllBone = RefreshBone(allBone);

        GameObject leftGo = CreateCopy(weaponPrefab, weaponNameAndId, ObjectType.Weapon);
        GameObject rightGo = CreateCopy(weaponPrefab, weaponNameAndId, ObjectType.Weapon);

        if (type != _lastRightHandType
            | _rightHandType == WeaponType.pole
            | _rightHandType == WeaponType.staff
            | _leftHandType == WeaponType.bow)
        {
            OnEquipAnimationRefresh?.Invoke(-1, weapon);
        }
        else
        {
            if (_leftHandWeapon == null | _leftHandShield == null) OnEquipAnimationRefresh?.Invoke(-1, weapon);
        }

        ActivateOtherGameObject(leftGo, type, true, refreshAllBone);
        ActivateOtherGameObject(rightGo, type, false, refreshAllBone);

        _lastRightHandType = type;
        //_lastShieldHandType = type;
    }
}
