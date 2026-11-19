using System;
using System.Collections.Generic;
using UnityEngine;

public static partial class L2OverheadAnchor
{
    /// <summary>
    /// Gap above the skull to the bottom of the name stack.
    /// Interlude pawn NameOffset is 6 UU; Fineage keeps a little more hair clearance.
    /// </summary>
    public const float NameOffsetMeters = 10f / 52.5f;

    /// <summary>Entity objectId → live head bone (sampled each paint, not a cached Y).</summary>
    static readonly Dictionary<int, Transform> _headBoneByEntity = new Dictionary<int, Transform>(128);

    /// <summary>Transform instance id → same, for lobby pawns without Entity.</summary>
    static readonly Dictionary<int, Transform> _headBoneByInstance = new Dictionary<int, Transform>(16);
}
