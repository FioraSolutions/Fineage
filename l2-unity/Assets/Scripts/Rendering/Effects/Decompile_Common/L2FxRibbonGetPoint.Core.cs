using UnityEngine;

public static partial class L2FxRibbonGetPoint
{
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
}
