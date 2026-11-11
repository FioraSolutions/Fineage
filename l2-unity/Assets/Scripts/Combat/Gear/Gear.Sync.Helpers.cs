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
}
