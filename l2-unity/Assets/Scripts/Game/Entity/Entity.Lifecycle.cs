using System;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public partial class Entity : MonoBehaviour
{
    public bool HasAnimator()
    {
        if (Animator == null)
            Animator = GetComponentInChildren<Animator>(true);
        return Animator != null && Animator.runtimeAnimatorController != null;
    }

    public Transform GetWeaponTransform()
    {
        if (_gear == null)
            return null;

        // weapon_shot.int [BOW] RH=-1 LH=0: the charge is attached to the left-hand bow, not Sword Bone.
        if (_gear.WeaponType == WeaponType.bow)
        {
            Transform bow = FirstWeaponMesh(_gear.GetAllTransformByLeftHand(new[] { Gear.weaponName }));
            if (bow != null)
                return bow;
        }

        Transform right = FirstWeaponMesh(_gear.GetAllTransformByRightHand(new[] { Gear.weaponName }));
        if (right != null)
            return right;

        return _gear.GetTransformRightHandBone();
    }

    static Transform FirstWeaponMesh(Transform[] found)
    {
        if (found == null)
            return null;

        for (int i = 0; i < found.Length; i++)
        {
            if (found[i] != null)
                return found[i];
        }

        return null;
    }
}
