using UnityEngine;

public static partial class L2FxRibbonGetPoint
{
    public static Vector3 EdgeMid(Vector3 a2, Vector3 a3)
    {
        return (a2 + a3) * 0.5f;
    }

    public static Edges LerpEdges(in Edges from, in Edges to, float t)
    {
        Edges e;
        e.A2 = Vector3.Lerp(from.A2, to.A2, t);
        e.A3 = Vector3.Lerp(from.A3, to.A3, t);
        e.A4 = Mathf.Lerp(from.A4, to.A4, t);
        return e;
    }

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
}
