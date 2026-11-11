using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public static partial class L2CelestialLut
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
            if (c.Length < 39)
            {
                continue;
            }

            Vector3 sunDirUe = new Vector3(Parse(c[4]), Parse(c[5]), Parse(c[6]));
            Vector3 moonDirUe = new Vector3(Parse(c[9]), Parse(c[10]), Parse(c[11]));
            Vector3 sunLocUe = new Vector3(Parse(c[20]), Parse(c[21]), Parse(c[22]));
            Vector3 moonLocUe = new Vector3(Parse(c[23]), Parse(c[24]), Parse(c[25]));

            Samples.Add(new Sample
            {
                tHours = Parse(c[1]),
                phase = int.Parse(c[2], CultureInfo.InvariantCulture),
                isDay = Parse(c[3]) > 0.5f,
                sunDirUnity = DirUeToUnity(sunDirUe),
                moonDirUnity = DirUeToUnity(moonDirUe),
                sunOffsetUnity = VectorUtils.ConvertPosToUnity(sunLocUe - SweepCameraUe),
                moonOffsetUnity = VectorUtils.ConvertPosToUnity(moonLocUe - SweepCameraUe),
                sunElevDeg = Parse(c[7]),
                moonElevDeg = Parse(c[12]),
                sunDrawScale = Parse(c[26]),
                moonDrawScale = Parse(c[27]),
                sunScale = Parse(c[28]),
                moonScale = Parse(c[29]),
                moonColor = new Color(Parse(c[36]) / 255f, Parse(c[37]) / 255f, Parse(c[38]) / 255f, 1f)
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
            Sample first = Samples[0];
            first.sampleA = 0;
            first.sampleB = 0;
            first.lerpU = 0f;
            return first;
        }

        if (i1 >= Samples.Count)
        {
            var last = Samples[Samples.Count - 1];
            var first = Samples[0];
            float span = (24f - last.tHours) + first.tHours;
            float u = span > 1e-6f ? (t - last.tHours) / span : 0f;
            return Lerp(last, first, u, t, Samples.Count - 1, 0);
        }

        var a = Samples[i1 - 1];
        var b = Samples[i1];
        float s = b.tHours - a.tHours;
        float k = s > 1e-6f ? (t - a.tHours) / s : 0f;
        return Lerp(a, b, k, t, i1 - 1, i1);
    }
}
