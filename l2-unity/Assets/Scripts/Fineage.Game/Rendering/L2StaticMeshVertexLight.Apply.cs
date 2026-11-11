using System;
using System.Globalization;
using UnityEngine;

public static partial class L2StaticMeshVertexLight
{
    public static float WorldRadiusUu(float lightRadius)
    {
        return (lightRadius + 1f) * 25f;
    }

    public static Vector3 ToUnityPosition(Vector3 l2)
    {
        return new Vector3(l2.y, l2.z, l2.x) * UuToMeters;
    }

    public static Vector3 ToUnityDirection(Vector3 l2)
    {
        return new Vector3(l2.y, l2.z, l2.x);
    }

    public static Vector3 RotatorToL2Direction(float pitch, float yaw)
    {
        float pr = pitch * Mathf.PI / 32768f;
        float yr = yaw * Mathf.PI / 32768f;
        float cp = Mathf.Cos(pr);
        return new Vector3(cp * Mathf.Cos(yr), cp * Mathf.Sin(yr), Mathf.Sin(pr));
    }

    public static Color LightPlane(int hue, int saturation, float brightness)
    {
        Vector3 chroma = Chroma(hue, saturation);
        const float storedScale = 0.547547f;
        float v = Mathf.Max(brightness, 0f) / 255f * storedScale;
        return new Color(chroma.x * v, chroma.y * v, chroma.z * v, 1f);
    }

    static float SampleHorizontal(Vector3 lightPos, float radius, Vector3 worldPos)
    {
        Vector3 delta = lightPos - worldPos;
        float dist = delta.magnitude;
        if (radius <= 1e-4f || dist >= radius)
            return 0f;
        float horiz = delta.x * delta.x + delta.z * delta.z;
        float atten = 1f - horiz / (radius * radius);
        if (atten <= 0f)
            return 0f;
        return atten * 2f;
    }

    static float SampleSqrtFalloff(Vector3 lightPos, float radius, Vector3 worldPos, Vector3 normal)
    {
        Vector3 delta = lightPos - worldPos;
        float dist = delta.magnitude;
        if (radius <= 1e-4f || dist >= radius || Vector3.Dot(normal, delta) <= 0f)
            return 0f;
        float under = 1.02f - dist / radius;
        if (under <= 0f)
            return 0f;
        return 2f * Mathf.Sqrt(under);
    }

    static float SampleSquaredFalloff(Vector3 lightPos, float radius, Vector3 worldPos, Vector3 normal)
    {
        Vector3 delta = lightPos - worldPos;
        float distSq = delta.sqrMagnitude;
        float radiusSq = radius * radius;
        if (radiusSq <= 1e-4f || distSq >= radiusSq || Vector3.Dot(normal, delta) <= 0f)
            return 0f;
        return 2f * (1.02f - distSq / radiusSq);
    }

    static Vector3 Chroma(int hue, int saturation)
    {
        float h = hue / 85f;
        float r;
        float g;
        float b;
        if (h <= 1f)
        {
            g = h;
            r = 1f - g;
            b = 0f;
        }
        else if (h <= 2f)
        {
            b = h - 1f;
            g = 1f - b;
            r = 0f;
        }
        else
        {
            r = h - 2f;
            b = 1f - r;
            g = 0f;
        }

        float sat = Mathf.Clamp01(saturation / 255f);
        r = Mathf.Lerp(r, 1f, sat);
        g = Mathf.Lerp(g, 1f, sat);
        b = Mathf.Lerp(b, 1f, sat);
        return new Vector3(r, g, b);
    }

    static int AddChannel(int current, float value)
    {
        if (value <= 0f)
            return current;
        int add = (int)value;
        int sum = current + add;
        return sum > 255 ? 255 : sum;
    }

    static string ActorName(string beginLine)
    {
        const string key = "Name=";
        int at = beginLine.IndexOf(key, StringComparison.Ordinal);
        if (at < 0)
            return "Light";
        return beginLine.Substring(at + key.Length).Trim();
    }

    static float ParseFloat(string line)
    {
        int at = line.IndexOf('=');
        if (at < 0)
            return 0f;
        float value;
        float.TryParse(line.Substring(at + 1), NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        return value;
    }

    static int ParseInt(string line)
    {
        return (int)ParseFloat(line);
    }

    static Vector3 ParseLocation(string line)
    {
        return new Vector3(Field(line, "X"), Field(line, "Y"), Field(line, "Z"));
    }

    static void ParseRotation(string line, out float pitch, out float yaw)
    {
        pitch = Field(line, "Pitch");
        yaw = Field(line, "Yaw");
    }

    static float Field(string line, string name)
    {
        int at = line.IndexOf(name + "=", StringComparison.Ordinal);
        if (at < 0)
            return 0f;
        at += name.Length + 1;
        int end = at;
        while (end < line.Length && line[end] != ',' && line[end] != ')')
            end++;
        float value;
        float.TryParse(line.Substring(at, end - at), NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        return value;
    }
}
