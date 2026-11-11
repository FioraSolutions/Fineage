using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

public static partial class MagicSkillUseTrace
{
    public static Span Measure(string name)
    {
        return new Span(name);
    }

    static void Dump(double total)
    {
        double accounted = 0.0;
        var text = new StringBuilder(256);
        text.Append("skill ").Append(_skillId);
        text.Append(" frame ").Append(UnityEngine.Time.frameCount);
        text.Append(" applyMs ").Append(total.ToString("0.0"));
        for (int i = 0; i < Rows.Count; i++)
        {
            Row row = Rows[i];
            if (row.Level == 0)
                accounted += row.Ms;
            text.Append("\n  ");
            for (int pad = 0; pad < row.Level; pad++)
                text.Append("  ");
            text.Append(row.Name).Append(" ").Append(row.Ms.ToString("0.0")).Append("ms");
        }

        double other = total - accounted;
        if (other > 0.5)
            text.Append("\n  other ").Append(other.ToString("0.0")).Append("ms");

        string line = text.ToString();
        try
        {
            File.AppendAllText(LogPath, line + "\n\n");
        }
        catch (Exception ex)
        {
            Debug.LogWarning("MagicSkillUseTrace file: " + ex.Message);
        }

        Debug.Log(line);
    }

    static double Elapsed(long started)
    {
        return (Stopwatch.GetTimestamp() - started) * 1000.0 / Stopwatch.Frequency;
    }
}
