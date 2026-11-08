using UnityEngine;

/// <summary>
/// Low-level Interlude capsule math (feet → capsule top). Prefer
/// <see cref="L2OverheadAnchor"/> for HeadDisplay / cast / TargetOverHead.
/// </summary>
public static class L2NameplateAnchor
{
    public const float DefaultCollisionHeightMeters = 0.46f;

    /// <summary>Deprecated alias of <see cref="L2OverheadAnchor.NameOffsetMeters"/> for legacy callers.</summary>
    public static float DefaultHeadHeightOffsetMeters => L2OverheadAnchor.NameOffsetMeters;

    /// <summary>
    /// World position at capsule top (+ <paramref name="headHeightOffset"/>).
    /// Pass cc/capsule null for User/Player/Npc — CC tops are unreliable mid-torso.
    /// </summary>
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

    /// <summary>
    /// Resolves CC/Capsule on <paramref name="target"/> then <see cref="GetHeadWorldPos"/>.
    /// Prefer explicit null CC for players — see <see cref="L2OverheadAnchor"/>.
    /// </summary>
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

    /// <summary>
    /// Half-height in Unity meters. npcgrp is already /52.5; UserInfoDto often raw UU.
    /// </summary>
    public static float CollisionHeightToUnityMeters(float collisionHeight)
    {
        if (collisionHeight <= 0.0001f)
            return DefaultCollisionHeightMeters;

        // Raw Interlude UU half-heights are typically ~3..40; converted meters are usually under ~1.
        if (collisionHeight > 2.5f)
            return collisionHeight / 52.5f;

        return collisionHeight;
    }

    public static float HeightFromFeet(float collisionHeight, float headHeightOffset)
    {
        float ch = CollisionHeightToUnityMeters(collisionHeight);
        return 2f * ch + headHeightOffset;
    }
}
