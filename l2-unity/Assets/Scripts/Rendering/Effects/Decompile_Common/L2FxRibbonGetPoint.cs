using UnityEngine;

/// <summary>
/// C# mirror of Decompile_Common/L2FxRibbonGetPoint.hlsl.
/// URibbonEmitter::GetNewRibbonPoint — live-verified Interlude RibbonSet (CoordSys==1).
/// Keep in sync with the HLSL module; do not fork formulas in effect scripts.
/// </summary>
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
    public static Edges GetPointCoordSys1(Vector3 pos, Vector3 normalUnit, float scaleRatio, float edgeRatio)
    {
        Vector3 n = normalUnit;
        float nLenSq = n.sqrMagnitude;
        if (nLenSq > 1e-12f)
        {
            n *= 1f / Mathf.Sqrt(nLenSq);
        }
        else
        {
            n = Vector3.right;
        }

        float s = scaleRatio;
        float r = edgeRatio;
        Edges e;
        e.A3 = pos - n * (s * r);
        e.A2 = pos + n * (s * (1f - r));
        e.A4 = s;
        return e;
    }

    /// <summary>
    /// Blade ends as ribbon cross-section (Unity Sword_Tip / Sword_Base).
    /// Matches live |a2-a3| == blade length. a2 = tip, a3 = base.
    /// </summary>
    public static Edges GetPointFromBladeEnds(Vector3 tipWorld, Vector3 baseWorld, float edgeRatio = 0.5f)
    {
        Vector3 tip = tipWorld;
        Vector3 hilt = baseWorld;
        Vector3 axis = tip - hilt;
        float len = axis.magnitude;
        if (len < 1e-6f)
        {
            Edges degenerated;
            degenerated.A2 = tip;
            degenerated.A3 = hilt;
            degenerated.A4 = 0f;
            return degenerated;
        }

        if (Mathf.Abs(edgeRatio - 0.5f) < 1e-4f)
        {
            Edges full;
            full.A2 = tip;
            full.A3 = hilt;
            full.A4 = len;
            return full;
        }

        Vector3 mid = (tip + hilt) * 0.5f;
        Vector3 n = axis / len;
        return GetPointCoordSys1(mid, n, len, edgeRatio);
    }

    /// <summary>
    /// World tip/hilt from weapon mesh local bounds along the longest axis,
    /// transformed through the MeshFilter (or SkinnedMeshRenderer) transform.
    /// SwordSetup often parents Sword_Tip/Base to the weapon root while writing
    /// mesh-local bounds — that under-spans the blade when the mesh is a child.
    /// </summary>
    public static bool TryGetMeshBladeEnds(Transform searchRoot, out Vector3 tipWorld, out Vector3 hiltWorld)
    {
        tipWorld = default;
        hiltWorld = default;
        if (searchRoot == null)
        {
            return false;
        }

        Transform meshXf = null;
        Bounds localBounds = default;
        bool found = false;

        MeshFilter mf = searchRoot.GetComponentInChildren<MeshFilter>();
        if (mf != null && mf.sharedMesh != null)
        {
            meshXf = mf.transform;
            localBounds = mf.sharedMesh.bounds;
            found = true;
        }
        else
        {
            SkinnedMeshRenderer smr = searchRoot.GetComponentInChildren<SkinnedMeshRenderer>();
            if (smr != null && smr.sharedMesh != null)
            {
                meshXf = smr.transform;
                localBounds = smr.sharedMesh.bounds;
                found = true;
            }
        }

        if (!found || meshXf == null)
        {
            return false;
        }

        Vector3 size = localBounds.size;
        Vector3 minLocal;
        Vector3 maxLocal;
        if (size.x >= size.y && size.x >= size.z)
        {
            minLocal = new Vector3(localBounds.min.x, localBounds.center.y, localBounds.center.z);
            maxLocal = new Vector3(localBounds.max.x, localBounds.center.y, localBounds.center.z);
        }
        else if (size.z >= size.x && size.z >= size.y)
        {
            minLocal = new Vector3(localBounds.center.x, localBounds.center.y, localBounds.min.z);
            maxLocal = new Vector3(localBounds.center.x, localBounds.center.y, localBounds.max.z);
        }
        else
        {
            minLocal = new Vector3(localBounds.center.x, localBounds.min.y, localBounds.center.z);
            maxLocal = new Vector3(localBounds.center.x, localBounds.max.y, localBounds.center.z);
        }

        // Convention: max = tip, min = hilt (matches SwordSetup max→tip naming intent).
        tipWorld = meshXf.TransformPoint(maxLocal);
        hiltWorld = meshXf.TransformPoint(minLocal);
        return (tipWorld - hiltWorld).sqrMagnitude > 1e-10f;
    }

    /// <summary>
    /// Keep a2/a3 from flipping end-for-end between samples (common when
    /// tip/base markers disagree with mesh-bounds orientation). Live ribbon
    /// width axis stays continuous; a flipped axis tears the strip.
    /// </summary>
    public static Edges StabilizeEdgePolarity(in Edges current, in Edges reference)
    {
        Vector3 curAxis = current.A2 - current.A3;
        Vector3 refAxis = reference.A2 - reference.A3;
        if (curAxis.sqrMagnitude < 1e-12f || refAxis.sqrMagnitude < 1e-12f)
        {
            return current;
        }

        if (Vector3.Dot(curAxis, refAxis) >= 0f)
        {
            return current;
        }

        Edges swapped;
        swapped.A2 = current.A3;
        swapped.A3 = current.A2;
        swapped.A4 = current.A4;
        return swapped;
    }

    /// <summary>
    /// Prefer proximity matching over axis-dot polarity (Dot&lt;0 false-swaps on &gt;90° swings).
    /// </summary>
    public static Edges StabilizeEdgeEndsByProximity(in Edges current, in Edges reference)
    {
        float keep =
            Vector3.Distance(current.A2, reference.A2) +
            Vector3.Distance(current.A3, reference.A3);
        float swap =
            Vector3.Distance(current.A2, reference.A3) +
            Vector3.Distance(current.A3, reference.A2);

        if (swap + 1e-5f < keep)
        {
            Edges swapped;
            swapped.A2 = current.A3;
            swapped.A3 = current.A2;
            swapped.A4 = current.A4;
            return swapped;
        }

        return current;
    }

}
