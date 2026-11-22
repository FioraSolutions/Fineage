using System.Globalization;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

public static partial class L2DayNightLut
{
    public static void LoadCsv(string csvText)
    {
        Samples.Clear();
        if (string.IsNullOrEmpty(csvText))
        {
            return;
        }

        var lines = csvText.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        for (int i = 1; i < lines.Length; i++)
        {
            var c = lines[i].Split(',');
            // step,t_hours,t01,phase,phaseName,envType,skyRGB,sunRGB,actorRGB,sky01,sun01,actor01,hex
            if (c.Length < 24)
            {
                continue;
            }

            Samples.Add(new Sample
            {
                tHours = Parse(c[1]),
                phase = int.Parse(c[3], CultureInfo.InvariantCulture),
                sky = new Color(Parse(c[15]), Parse(c[16]), Parse(c[17]), 1f),
                sun = new Color(Parse(c[18]), Parse(c[19]), Parse(c[20]), 1f),
                actorAmbient = new Color(Parse(c[21]), Parse(c[22]), Parse(c[23]), 1f)
            });
        }

        Samples.Sort((a, b) => a.tHours.CompareTo(b.tHours));
    }

    public static Sample SampleAt(float worldHours)
    {
        if (!IsReady)
        {
            return default;
        }

        float t = worldHours % 24f;
        if (t < 0f)
        {
            t += 24f;
        }

        int i1 = 0;
        while (i1 < Samples.Count && Samples[i1].tHours < t)
        {
            i1++;
        }

        if (i1 <= 0)
        {
            return Samples[0];
        }

        if (i1 >= Samples.Count)
        {
            return Samples[Samples.Count - 1];
        }

        var a = Samples[i1 - 1];
        var b = Samples[i1];
        float span = b.tHours - a.tHours;
        float u = span > 1e-6f ? (t - a.tHours) / span : 0f;

        return new Sample
        {
            tHours = t,
            phase = u < 0.5f ? a.phase : b.phase,
            sky = Color.Lerp(a.sky, b.sky, u),
            sun = Color.Lerp(a.sun, b.sun, u),
            actorAmbient = Color.Lerp(a.actorAmbient, b.actorAmbient, u)
        };
    }
}
