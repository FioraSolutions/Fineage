using UnityEngine;

public static partial class L2FxRibbonGetPoint
{
    public static Vector3 EdgeMid(Vector3 a2, Vector3 a3)
    {
        return (a2 + a3) * 0.5f;
    }

    /// <summary>
    /// Linear sheet between two cross-sections (a2/a3/a4).
    /// Prefer <see cref="LerpEdgesArc"/> for swing trails — linear chords look
    /// "скошенными" on large per-frame tip motion.
    /// </summary>
    public static Edges LerpEdges(in Edges from, in Edges to, float t)
    {
        Edges e;
        e.A2 = Vector3.Lerp(from.A2, to.A2, t);
        e.A3 = Vector3.Lerp(from.A3, to.A3, t);
        e.A4 = Mathf.Lerp(from.A4, to.A4, t);
        return e;
    }

    /// <summary>
    /// Arc sheet: Slerp tip and base around <paramref name="pivot"/> (weapon/hand).
    /// Unity only has one animated pose per frame; linear Lerp of endpoints
    /// draws a chord. Pivot-Slerp approximates the real swing arc so a top-down
    /// cut reads as a curve instead of a flat cut.
    /// </summary>
    public static Edges LerpEdgesArc(in Edges from, in Edges to, float t, Vector3 pivot)
    {
        t = Mathf.Clamp01(t);
        Vector3 a2 = PivotSlerp(from.A2, to.A2, pivot, t);
        Vector3 a3 = PivotSlerp(from.A3, to.A3, pivot, t);
        float width = Mathf.Lerp(
            Mathf.Max(1e-6f, from.A4),
            Mathf.Max(1e-6f, to.A4),
            t);

        Vector3 mid = EdgeMid(a2, a3);
        Vector3 axis = a2 - a3;
        float axisLen = axis.magnitude;
        if (axisLen > 1e-6f)
        {
            axis /= axisLen;
            // Keep CS1-style R=0.5 cross-section at interpolated ScaleRatio.
            a2 = mid + axis * (width * 0.5f);
            a3 = mid - axis * (width * 0.5f);
        }

        Edges e;
        e.A2 = a2;
        e.A3 = a3;
        e.A4 = width;
        return e;
    }

    static Vector3 PivotSlerp(Vector3 from, Vector3 to, Vector3 pivot, float t)
    {
        Vector3 a = from - pivot;
        Vector3 b = to - pivot;
        float ra = a.magnitude;
        float rb = b.magnitude;
        if (ra < 1e-5f || rb < 1e-5f)
        {
            return Vector3.Lerp(from, to, t);
        }

        Vector3 dir = Vector3.Slerp(a / ra, b / rb, t);
        return pivot + dir * Mathf.Lerp(ra, rb, t);
    }

    /// <summary>
    /// Sheet count from the longest of tip/base/mid travel (tip often moves
    /// farther than mid on a swing).
    /// </summary>
    public static int CountInterpSheetsForEdges(
        in Edges from,
        in Edges to,
        float maxSegmentMeters,
        int maxSheets)
    {
        float tipD = Vector3.Distance(from.A2, to.A2);
        float baseD = Vector3.Distance(from.A3, to.A3);
        float midD = Vector3.Distance(EdgeMid(from.A2, from.A3), EdgeMid(to.A2, to.A3));
        float path = Mathf.Max(tipD, Mathf.Max(baseD, midD));
        return CountInterpSheets(path, maxSegmentMeters, maxSheets);
    }

    /// <summary>
    /// How many intermediate sheets to insert so consecutive midpoints are
    /// at most <paramref name="maxSegmentMeters"/> apart.
    /// </summary>
    public static int CountInterpSheets(float midDistanceMeters, float maxSegmentMeters, int maxSheets)
    {
        if (midDistanceMeters <= 1e-6f || maxSegmentMeters <= 1e-6f)
        {
            return 0;
        }

        int segments = Mathf.CeilToInt(midDistanceMeters / maxSegmentMeters);
        int sheets = Mathf.Max(0, segments - 1);
        return Mathf.Min(sheets, Mathf.Max(0, maxSheets));
    }
}
