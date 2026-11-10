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
    public  void UnequipWeapon(bool leftSlot , int weaponId, bool lrDestroy = false)
    {
        string weapondNameId = weaponName + weaponId;

        if (lrDestroy)
        {
            UnequipSingleWeapon(true, weaponId, weapondNameId);
            UnequipSingleWeapon(false, weaponId, weapondNameId);
        }
        else
        {
            UnequipSingleWeapon(leftSlot, weaponId, weapondNameId);
        }

    }

    private void UnequipSingleWeapon(bool leftSlot , int weaponId , string weapondNameId)
    {

        Transform weapon = (leftSlot ? GetLeftHandBone() : GetRightHandBone())?.Find(weapondNameId);


        if (weapon != null)
        {
            string origWeaponPrefabName = GetWeaponModelName(weaponId);

            DestroyObject(weapon.gameObject, origWeaponPrefabName);
            RefreshLastType(leftSlot, weaponId, WeaponType.none);
            RefreshHandWeapon(leftSlot, null, weaponId);


            int findCount = FindAllWeaponCount(weaponName, shieldName);

            if (findCount == 0)
            {
                RefreshData(leftSlot, null);
                _lastWeaponAnim = _weaponAnim;
                UpdateWeaponType(WeaponType.hand);
                OnUnequipAnimationRefresh?.Invoke("");
                // Debug.Log("Destroy request item id 3 " + weaponId);
            }
        }
    }

    private void RefreshLastType(bool leftslot , int weaponId, WeaponType type)
    {
        if (!leftslot)
        {
            if(_rightHandWeapon != null)
            {
                if(_rightHandWeapon.Id == weaponId)
                {
                    _lastRightHandType = type;
                }
            }
        }

    }

    public void UnequipShield(int shieldId)
    {
        string shieldNameId = shieldName + shieldId;
        Transform shield = GetShieldBone()?.Find(shieldNameId);



        if (shield != null)
        {
            DestroyObject(shield.gameObject, _origShieldPrefabName);

            RefreshHandShield(null);
            int findCount = FindAllWeaponCount(weaponName, shieldName);
            //if no equip sword and no equip shield. findCount = 1 > then remove current shield 
            if (findCount == 0)
            {
                RefreshDataShield(null);
                UpdateWeaponType(WeaponType.hand);
                OnUnequipAnimationRefresh?.Invoke("");
            }

        }
    }

    private void DestroyObject(GameObject go , string origPrefabName)
    {
        IPoolManager pool = ObjectPoolPacketServices.Pool;
        if (pool != null)
        {
            go.name = origPrefabName;
            if (!pool.ReturnToPool(ObjectType.Weapon, go))
            {
                Destroy(go);
            }
        }
        else
        {
            Destroy(go);
        }
    }

    public bool IsWeaponEquipped(int weaponId, bool leftSlot)
    {
        Weapon weapon = leftSlot ? _leftHandWeapon : _rightHandWeapon;
        if (weapon == null) return false;
        return weapon.Id == weaponId;
    }

    public bool IsWeaponEquippedInBothHands(int weaponId)
    {

        bool inLeftHand = _leftHandWeapon != null && _leftHandWeapon.Id == weaponId;
        bool inRightHand = _rightHandWeapon != null && _rightHandWeapon.Id == weaponId;

    
        return inLeftHand && inRightHand;
    }

    public bool IsShieldEquipped(int weaponId)
    {
        Weapon weapon = _leftHandShield;
        return weapon != null && weapon.Id == weaponId;
    }


    private void RefreshData(bool leftSlot , Weapon weapon)
    {
        // Updating weapon type
        if (leftSlot)
        {
            _leftHandWeapon = weapon;
            var typeL = (weapon == null) ? WeaponType.hand : weapon.Weapongrp.WeaponType;
            _leftHandType = typeL;
        }
        else
        {
            _rightHandWeapon = weapon;
            var typeR = (weapon == null) ? WeaponType.hand : weapon.Weapongrp.WeaponType;
            _rightHandType = typeR;
        }
    }

    private void RefreshHandWeapon(bool leftSlot, Weapon weapon, int weaponId)
    {
        var targetWeapon = leftSlot ? _leftHandWeapon : _rightHandWeapon;
        if (targetWeapon?.Id == weaponId)
        {
            if (leftSlot) _leftHandWeapon = weapon;
            else _rightHandWeapon = weapon;
        }
    }


    private void RefreshDataShield(Weapon weapon , WeaponType weaponType = WeaponType.hand)
    {
        _leftHandShield = weapon;
        _leftHandWeapon = weapon;
        _leftHandType = weaponType;
    }

    private void RefreshHandShield(Weapon weapon)
    {
        _leftHandShield = weapon;
        _leftHandWeapon = weapon;
    }


}
