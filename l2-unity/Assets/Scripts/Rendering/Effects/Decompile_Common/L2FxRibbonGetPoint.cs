using UnityEngine;

public static partial class L2FxRibbonGetPoint
{
    public struct Edges
    {
        public Vector3 A2;
        public Vector3 A3;
        public float A4;
    }

    /// <summary>
    /// CoordSys == 1: a3 = Pos - N*S*R ; a2 = Pos + N*S*(1-R) ; |a2-a3| == S.
    /// </summary>

    /// <summary>
    /// Blade ends as ribbon cross-section (Unity Sword_Tip / Sword_Base).
    /// Matches live |a2-a3| == blade length. a2 = tip, a3 = base.
    /// </summary>

    /// <summary>
    /// World tip/hilt from weapon mesh local bounds along the longest axis,
    /// transformed through the MeshFilter (or SkinnedMeshRenderer) transform.
    /// SwordSetup often parents Sword_Tip/Base to the weapon root while writing
    /// mesh-local bounds — that under-spans the blade when the mesh is a child.
    /// </summary>

    /// <summary>
    /// Keep a2/a3 from flipping end-for-end between samples (common when
    /// tip/base markers disagree with mesh-bounds orientation). Live ribbon
    /// width axis stays continuous; a flipped axis tears the strip.
    /// </summary>

    /// <summary>
    /// Prefer proximity matching over axis-dot polarity (Dot&lt;0 false-swaps on &gt;90° swings).
    /// </summary>

}
