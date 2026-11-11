using UnityEngine;

public static partial class L2FxRibbonGetPoint
{
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
