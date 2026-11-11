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

    /// <summary>Entity objectId → local Y of head (root-local).</summary>
    static readonly Dictionary<int, float> _localHeadYByEntity = new Dictionary<int, float>(128);

    /// <summary>Transform instance id → same, for lobby pawns without Entity.</summary>
    static readonly Dictionary<int, float> _localHeadYByInstance = new Dictionary<int, float>(16);




    /// <summary>
    /// World point at the bottom of the HeadDisplay stack (under name / cast gauge).
    /// Never throws on the paint path (#930): biped head in band → skinned top → constant.
    /// </summary>

    /// <summary>Lobby / no-Entity path. Never throws (#930).</summary>




    /// <summary>Pick the highest Bip01 Head match (some meshes expose a near-root stub first).</summary>
}
