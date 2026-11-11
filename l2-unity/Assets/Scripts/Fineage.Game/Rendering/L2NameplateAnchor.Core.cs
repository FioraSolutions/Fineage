using UnityEngine;

public static partial class L2NameplateAnchor
{
    public static Vector3 GetHeadWorldPos(
        Transform target,
        CharacterController cc,
        CapsuleCollider capsule,
        float collisionHeight,
        float headHeightOffset)
    {
        if (target == null)
            return Vector3.zero;

        if (cc != null)
        {
            Vector3 localTop = cc.center + Vector3.up * (cc.height * 0.5f);
            return target.TransformPoint(localTop) + Vector3.up * headHeightOffset;
        }

        if (capsule != null)
        {
            Vector3 localTop = capsule.center + Vector3.up * (capsule.height * 0.5f);
            return target.TransformPoint(localTop) + Vector3.up * headHeightOffset;
        }

        // Feet GO + 2×CH: UE Location is capsule center (= feet+CH), name base = Loc+CH.
        return target.position + Vector3.up * HeightFromFeet(collisionHeight, headHeightOffset);
    }

    public static Vector3 GetHeadWorldPos(
        Transform target,
        float collisionHeight,
        float headHeightOffset)
    {
        if (target == null)
            return Vector3.zero;

        CharacterController cc = target.GetComponent<CharacterController>();
        if (cc == null)
            cc = target.GetComponentInChildren<CharacterController>();

        CapsuleCollider capsule = null;
        if (cc == null)
        {
            capsule = target.GetComponent<CapsuleCollider>();
            if (capsule == null)
                capsule = target.GetComponentInChildren<CapsuleCollider>();
        }

        return GetHeadWorldPos(target, cc, capsule, collisionHeight, headHeightOffset);
    }
}
