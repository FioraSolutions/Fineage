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
}
