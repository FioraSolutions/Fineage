using UnityEngine;

public static partial class L2NameplateAnchor
{
    public const float DefaultCollisionHeightMeters = 0.46f;

    /// <summary>
    /// Deprecated alias of Interlude NameOffset (10 UU). Keep in sync with
    /// <c>L2OverheadAnchor.NameOffsetMeters</c> in Assembly-CSharp.
    /// </summary>
    public const float DefaultHeadHeightOffsetMeters = 10f / 52.5f;

    /// <summary>
    /// World position at capsule top (+ <paramref name="headHeightOffset"/>).
    /// Pass cc/capsule null for User/Player/Npc — CC tops are unreliable mid-torso.
    /// </summary>

    /// <summary>
    /// Resolves CC/Capsule on <paramref name="target"/> then <see cref="GetHeadWorldPos"/>.
    /// Prefer explicit null CC for players — see <see cref="L2OverheadAnchor"/>.
    /// </summary>

    /// <summary>
    /// Half-height in Unity meters. npcgrp is already /52.5; UserInfoDto often raw UU.
    /// </summary>

}
