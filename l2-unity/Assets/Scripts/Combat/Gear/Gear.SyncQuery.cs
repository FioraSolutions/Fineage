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
