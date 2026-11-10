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

    bool HasOrphanHandMeshes(int wantRight, int wantLeft, int wantShield)
    {
        return BoneHasUnexpectedItem(GetRightHandBone(), weaponName, wantRight) ||
            BoneHasUnexpectedItem(GetLeftHandBone(), weaponName, wantLeft) ||
            BoneHasUnexpectedItem(GetShieldBone(), shieldName, wantShield);
    }

    bool HasExpectedHandMeshes(int wantRight, int wantLeft, int wantShield)
    {
        return BoneHasExpectedItem(GetRightHandBone(), weaponName, wantRight) &&
            BoneHasExpectedItem(GetLeftHandBone(), weaponName, wantLeft) &&
            BoneHasExpectedItem(GetShieldBone(), shieldName, wantShield);
    }

    static bool BoneHasExpectedItem(Transform bone, string prefix, int expectedId)
    {
        if (expectedId == 0)
            return true;

        Transform child = bone?.Find(prefix + expectedId);
        return child != null && child.gameObject.activeSelf;
    }

    bool BoneHasUnexpectedItem(Transform bone, string prefix, int allowedId)
    {
        if (bone == null)
            return false;

        int matching = 0;
        for (int i = 0; i < bone.childCount; i++)
        {
            Transform child = bone.GetChild(i);
            if (child == null || !child.name.StartsWith(prefix, StringComparison.Ordinal))
                continue;

            int itemId = 0;
            int.TryParse(child.name.Substring(prefix.Length), out itemId);
            if (allowedId == 0 || itemId != allowedId)
                return true;
            matching++;
            if (matching > 1)
                return true;
        }
        return false;
    }

    void ClearWeaponMeshesOnBone(Transform bone)
    {
        ClearPrefixedMeshesOnBone(bone, weaponName);
    }

    void ClearShieldMeshesOnBone(Transform bone)
    {
        ClearPrefixedMeshesOnBone(bone, shieldName);
    }

    void ClearPrefixedMeshesOnBone(Transform bone, string prefix)
    {
        if (bone == null)
            return;

        for (int i = bone.childCount - 1; i >= 0; i--)
        {
            Transform child = bone.GetChild(i);
            if (child == null || !child.name.StartsWith(prefix, StringComparison.Ordinal))
                continue;

            int itemId = 0;
            string suffix = child.name.Substring(prefix.Length);
            int.TryParse(suffix, out itemId);
            DestroyObject(child.gameObject, GetWeaponModelName(itemId));
        }
    }

    private IEnumerable<Transform> FindTransformsWithName(Transform parent, string pattern)
    {
        if (parent == null) yield break;

    
        if (parent.name.Contains(pattern))
        {
            yield return parent;
        }

 
        foreach (Transform child in parent)
        {
            foreach (Transform match in FindTransformsWithName(child, pattern))
            {
                yield return match;
            }
        }
    }
    private int FindAllWeaponCount(string allWeapons , string allShields)
    {
        var allWeaponsFound = new List<Transform>();
        allWeaponsFound.AddRange(FindTransformsWithName(GetLeftHandBone(), allWeapons));
        allWeaponsFound.AddRange(FindTransformsWithName(GetRightHandBone(), allWeapons));

        var allShieldsFound = FindTransformsWithName(GetShieldBone(), allShields).ToList();

        return allWeaponsFound.Count + allShieldsFound.Count;
    }

    public Vector3 GetPositionRightHand()
    {
        Transform hand = GetRightHandBone();
        return hand != null ? hand.position : transform.position;
    }

    public GameObject GetGoEtcItem()
    {
        return _goCurrentEtcItem;
    }

    public Transform FindRecursiveBone(string boneName)
    {
        return transform.FindRecursive(boneName);
    }

    public Transform[] GetAllTransformByRightHand(string[] arrNameGameObject)
    {
        Transform[] transfroms = new Transform[arrNameGameObject.Length];
        for (int i=0; i < arrNameGameObject.Length; i++)
        {
            string name =  arrNameGameObject[i];
            IEnumerable<Transform> result = FindTransformsWithName(GetRightHandBone(), name);
            if (result.GetEnumerator().MoveNext())
            {
                transfroms[i] = result.First();
            }

        }

        return transfroms;
    }

    public Transform[] GetAllTransformByLeftHand(string[] arrNameGameObject)
    {
        Transform[] transforms = new Transform[arrNameGameObject.Length];
        for (int i = 0; i < arrNameGameObject.Length; i++)
        {
            foreach (Transform match in FindTransformsWithName(GetLeftHandBone(), arrNameGameObject[i]))
            {
                transforms[i] = match;
                break;
            }
        }

        return transforms;
    }

    public bool IsTwoHandedEquipped()
    {
        return _rightHandType == WeaponType.bigword || 
               _rightHandType == WeaponType.pole ||    
               _rightHandType == WeaponType.staff ||  
               _rightHandType == WeaponType.bigblunt;  
    }
}
