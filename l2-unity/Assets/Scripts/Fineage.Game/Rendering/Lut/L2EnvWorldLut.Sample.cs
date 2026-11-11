using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public static partial class L2EnvWorldLut
{
    public static Sample SampleAt(float worldHours)
    {
        float t = Repeat24(worldHours);
        if (SweepKeys.Count >= 2)
            return SampleSweep(t);

        return new Sample
        {
            terrainAmbient = SampleColor(TerrainAmbient, t),
            staticMeshAmbient = SampleColor(StaticMeshAmbient, t),
            bspAmbient = SampleColor(BspAmbient, t),
            hsvTerrainLight = SampleHsv(HsvTerrainLight, t),
            hsvStaticMeshLight = Color.black
        };
    }

    static Sample SampleSweep(float t)
    {
        int i1 = 0;
        while (i1 < SweepKeys.Count && SweepKeys[i1].tHours < t)
            i1++;

        SweepKey a;
        SweepKey b;
        float u;
        if (i1 <= 0 || i1 >= SweepKeys.Count)
        {
            a = SweepKeys[SweepKeys.Count - 1];
            b = SweepKeys[0];
            float span = (24f - a.tHours) + b.tHours;
            float along = t >= a.tHours ? (t - a.tHours) : ((24f - a.tHours) + t);
            u = span > 1e-6f ? along / span : 0f;
        }
        else
        {
            a = SweepKeys[i1 - 1];
            b = SweepKeys[i1];
            float span = b.tHours - a.tHours;
            u = span > 1e-6f ? (t - a.tHours) / span : 0f;
        }

        return new Sample
        {
            terrainAmbient = Color.Lerp(a.terrain, b.terrain, u),
            staticMeshAmbient = Color.Lerp(a.staticMesh, b.staticMesh, u),
            bspAmbient = Color.Lerp(a.bsp, b.bsp, u),
            hsvTerrainLight = Color.Lerp(a.hsvTerrain, b.hsvTerrain, u),
            hsvStaticMeshLight = Color.Lerp(a.hsvStatic, b.hsvStatic, u)
        };
    }

    static Color SampleColor(List<KeyColor> keys, float t)
    {
        if (keys == null || keys.Count == 0)
            return Color.black;
        if (keys.Count == 1)
            return keys[0].color;

        int i1 = 0;
        while (i1 < keys.Count && keys[i1].tHours < t)
            i1++;

        if (i1 <= 0)
            return keys[0].color;
        if (i1 >= keys.Count)
            return keys[keys.Count - 1].color;

        KeyColor a = keys[i1 - 1];
        KeyColor b = keys[i1];
        float span = b.tHours - a.tHours;
        float u = span > 1e-6f ? (t - a.tHours) / span : 0f;
        return Color.Lerp(a.color, b.color, u);
    }

    static float SampleFloat(List<KeyFloat> keys, float t)
    {
        if (keys == null || keys.Count == 0)
            return DefaultTerrainIntensity01;
        if (keys.Count == 1)
            return keys[0].value;

        int i1 = 0;
        while (i1 < keys.Count && keys[i1].tHours < t)
            i1++;

        KeyFloat a;
        KeyFloat b;
        float u;
        if (i1 <= 0 || i1 >= keys.Count)
        {
            a = keys[keys.Count - 1];
            b = keys[0];
            float span = (24f - a.tHours) + b.tHours;
            float along = t >= a.tHours ? (t - a.tHours) : ((24f - a.tHours) + t);
            u = span > 1e-6f ? along / span : 0f;
        }
        else
        {
            a = keys[i1 - 1];
            b = keys[i1];
            float span = b.tHours - a.tHours;
            u = span > 1e-6f ? (t - a.tHours) / span : 0f;
        }

        return Mathf.Lerp(a.value, b.value, u);
    }

    static Color SampleHsv(List<KeyHsv> keys, float t)
    {
        if (keys == null || keys.Count == 0)
            return Color.black;
        if (keys.Count == 1)
            return HsvToHdr(keys[0]);

        int i1 = 0;
        while (i1 < keys.Count && keys[i1].tHours < t)
            i1++;

        if (i1 <= 0)
            return HsvToHdr(keys[0]);
        if (i1 >= keys.Count)
            return HsvToHdr(keys[keys.Count - 1]);

        KeyHsv a = keys[i1 - 1];
        KeyHsv b = keys[i1];
        float span = b.tHours - a.tHours;
        float u = span > 1e-6f ? (t - a.tHours) / span : 0f;
        var mix = new KeyHsv
        {
            tHours = t,
            hue = Mathf.Lerp(a.hue, b.hue, u),
            sat = Mathf.Lerp(a.sat, b.sat, u),
            bri = Mathf.Lerp(a.bri, b.bri, u)
        };
        return HsvToHdr(mix);
    }

    static Color HsvToHdr(KeyHsv key)
    {
        float h = Mathf.Repeat(key.hue / 255f, 1f);
        float s = Mathf.Clamp01(key.sat / 255f);
        float v = key.bri / 255f;
        return Color.HSVToRGB(h, s, v, true);
    }

    static Color ByteRgb(string r, string g, string b)
    {
        return new Color(Parse(r) / 255f, Parse(g) / 255f, Parse(b) / 255f, 1f);
    }

    static void SortByTime(List<KeyColor> keys)
    {
        keys.Sort((a, b) => a.tHours.CompareTo(b.tHours));
    }

    static float Repeat24(float hours)
    {
        float t = hours % 24f;
        if (t < 0f)
            t += 24f;
        return t;
    }
}
