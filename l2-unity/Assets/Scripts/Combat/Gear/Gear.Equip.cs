using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ModelTable;
using static Unity.Burst.Intrinsics.Arm;
using static UnityEngine.Rendering.DebugUI;

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
        WeaponType type = WeaponType.none;
        RefreshDataShield(weapon , WeaponType.shield);
        UpdateWeaponType(type);
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

    private Transform[] RefreshBone(Transform[] allBone)
    {
        allBone[0] = GetShieldBone();
        allBone[1] = GetLeftHandBone();
        allBone[2] = GetLeftHandBone();
        allBone[3] = GetRightHandBone();
        return allBone;
    }

    protected virtual Transform GetLeftHandBone() {
        if (_leftHandBone == null) {
            _leftHandBone = transform.FindRecursive("Bow Bone");
        }
        return _leftHandBone;
    }

    public Transform GetSpineBone()
    {
        if (_spineBone == null)
        {
            _spineBone = transform.FindRecursive("Bip01_Spine1");
        }
        return _spineBone;
    }

    public Transform GetRightUpperArm()
    {
        if (_rightUpperArm == null)
        {
            _rightUpperArm = transform.FindRecursive("Bip01_R_UpperArm");
        }
        return _rightUpperArm;
    }

    public Transform GetTransformRightHandBone()
    {
        return GetRightHandBone();
    }

    public Transform GetTransformLeftHandBone()
    {
        return GetLeftHandBone();
    }

    protected virtual Transform GetRightHandBone() {
        if (_rightHandBone == null) {
            _rightHandBone = transform.FindRecursive("Sword Bone");
        }
        return _rightHandBone;
    }

    protected virtual Transform GetShieldBone() {
        if (_shieldBone == null) {
            _shieldBone = transform.FindRecursive("Shield Bone");
        }
        return _shieldBone;
    }

    public float GetWeaponRange() {
        return VectorUtils.ConvertL2UuToMeters(_weaponRange);
    }
}
