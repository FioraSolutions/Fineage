using System.Collections.Generic;
using UnityEngine;
using System.Text;

public sealed partial class L2RibbonEmitter : EffectPart
{
    private void LogPointsDump()
    {
        int n = _points.Count;
        if (n == 0)
        {
            Log("POINTS empty");
            return;
        }

        System.Text.StringBuilder sb = new System.Text.StringBuilder(512);
        sb.Append("POINTS n=").Append(n)
            .Append(" space=").Append(_storeInOwnerLocal ? "ownerLocal" : "world");

        int show = Mathf.Min(4, n);
        for (int i = 0; i < show; i++)
        {
            AppendPoint(sb, "H" + i, _points[i]);
        }

        if (n > show + 1)
        {
            sb.Append(" ...");
            AppendPoint(sb, "T0", _points[n - 1]);
            if (n > show + 2)
            {
                AppendPoint(sb, "T1", _points[n - 2]);
            }
        }
        else if (n > show)
        {
            AppendPoint(sb, "T0", _points[n - 1]);
        }

        // Segment lengths head→next few (detect jumps vs L2 continuous path).
        sb.Append(" seg=");
        int segShow = Mathf.Min(6, n - 1);
        for (int i = 0; i < segShow; i++)
        {
            float seg = Vector3.Distance(
                L2FxRibbonGetPoint.EdgeMid(_points[i].A2, _points[i].A3),
                L2FxRibbonGetPoint.EdgeMid(_points[i + 1].A2, _points[i + 1].A3));
            if (i > 0)
            {
                sb.Append(',');
            }

            sb.Append(seg.ToString("F3"));
        }

        Log(sb.ToString());
    }

    private static void AppendPoint(System.Text.StringBuilder sb, string label, RibbonPoint p)
    {
        Vector3 mid = L2FxRibbonGetPoint.EdgeMid(p.A2, p.A3);
        float w = Vector3.Distance(p.A2, p.A3);
        sb.Append(' ').Append(label)
            .Append(" a2=").Append(Fmt(p.A2))
            .Append(" a3=").Append(Fmt(p.A3))
            .Append(" mid=").Append(Fmt(mid))
            .Append(" a4=").Append(p.A4.ToString("F3"))
            .Append(" w=").Append(w.ToString("F3"));
    }

    private static string Fmt(Vector3 v)
    {
        return $"({v.x:F3},{v.y:F3},{v.z:F3})";
    }

    private void Log(string message)
    {
        if (!_debugLog)
        {
            return;
        }

        // Debug.Log($"[L2RibbonEmitter] {name} {message}", this);
    }
}
