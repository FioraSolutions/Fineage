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
