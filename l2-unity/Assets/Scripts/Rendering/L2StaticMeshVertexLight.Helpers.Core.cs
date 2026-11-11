using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public static partial class L2StaticMeshVertexLight
{
    public static float SampleIntensity(Light light, Vector3 worldPos, Vector3 worldNormal)
    {
        Vector3 normal = worldNormal.sqrMagnitude > 1e-8f ? worldNormal.normalized : Vector3.up;
        switch (light.Type)
        {
            case 19:
                return SampleDirectional(light.Direction, normal);
            case 17:
                return SampleHorizontal(light.Position, light.Radius, worldPos);
            case 13:
                return SampleSqrtFalloff(light.Position, light.Radius, worldPos, normal);
            case 20:
                return SampleSquaredFalloff(light.Position, light.Radius, worldPos, normal);
            case 8:
            case 12:
                return SampleSpot(light, worldPos, normal);
            default:
                return SamplePoint(light.Position, light.Radius, worldPos, normal);
        }
    }

    public static Color32[] Paint(Vector3[] localPos, Vector3[] localNormal, Matrix4x4 localToWorld, IList<Light> lights, float meshBrightness)
    {
        int count = localPos != null ? localPos.Length : 0;
        var colors = new Color32[count];
        if (count == 0 || lights == null || lights.Count == 0)
        {
            for (int i = 0; i < count; i++)
                colors[i] = new Color32(0, 0, 0, 255);
            return colors;
        }

        float brightness = meshBrightness > 0f ? meshBrightness : 1f;
        for (int v = 0; v < count; v++)
        {
            Vector3 world = localToWorld.MultiplyPoint3x4(localPos[v]);
            Vector3 normal = localNormal != null && v < localNormal.Length
                ? localToWorld.MultiplyVector(localNormal[v])
                : Vector3.up;
            int r = 0;
            int g = 0;
            int b = 0;
            for (int i = 0; i < lights.Count; i++)
            {
                Light light = lights[i];
                float intensity = SampleIntensity(light, world, normal);
                if (intensity <= 0f)
                    continue;
                float scale = brightness * 0.5f * intensity * 255f;
                r = AddChannel(r, light.Color.r * scale);
                g = AddChannel(g, light.Color.g * scale);
                b = AddChannel(b, light.Color.b * scale);
            }

            colors[v] = new Color32((byte)r, (byte)g, (byte)b, 255);
        }

        return colors;
    }

    public static List<Light> LoadMapLights(string t3dPath)
    {
        var lights = new List<Light>();
        if (string.IsNullOrEmpty(t3dPath) || !File.Exists(t3dPath))
            return lights;

        using (var reader = new StreamReader(t3dPath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string trimmed = line.Trim();
                bool sun = trimmed.StartsWith("Begin Actor Class=NMovableSunLight ", StringComparison.Ordinal);
                bool point = trimmed.StartsWith("Begin Actor Class=Light ", StringComparison.Ordinal);
                if (!sun && !point)
                    continue;

                string name = ActorName(trimmed);
                float brightness = 64f;
                float radius = 64f;
                int hue = 0;
                int saturation = 255;
                int cone = 128;
                bool spotlight = false;
                bool disabled = false;
                Vector3 location = Vector3.zero;
                float pitch = 0f;
                float yaw = 0f;
                bool hasLocation = false;

                while ((line = reader.ReadLine()) != null && line.IndexOf("End Actor", StringComparison.Ordinal) < 0)
                {
                    trimmed = line.Trim();
                    if (trimmed.StartsWith("LightBrightness=", StringComparison.Ordinal))
                        brightness = ParseFloat(trimmed);
                    else if (trimmed.StartsWith("LightRadius=", StringComparison.Ordinal))
                        radius = ParseFloat(trimmed);
                    else if (trimmed.StartsWith("LightHue=", StringComparison.Ordinal))
                        hue = ParseInt(trimmed);
                    else if (trimmed.StartsWith("LightSaturation=", StringComparison.Ordinal))
                        saturation = ParseInt(trimmed);
                    else if (trimmed.StartsWith("LightCone=", StringComparison.Ordinal))
                        cone = ParseInt(trimmed);
                    else if (trimmed.StartsWith("LightEffect=LE_Spotlight", StringComparison.Ordinal))
                        spotlight = true;
                    else if (trimmed.StartsWith("LightType=LT_None", StringComparison.Ordinal))
                        disabled = true;
                    else if (trimmed.StartsWith("Location=", StringComparison.Ordinal))
                    {
                        location = ParseLocation(trimmed);
                        hasLocation = true;
                    }
                    else if (trimmed.StartsWith("Rotation=", StringComparison.Ordinal))
                        ParseRotation(trimmed, out pitch, out yaw);
                }

                if (disabled || !hasLocation)
                    continue;

                int type = sun ? 19 : (spotlight ? 12 : 0);
                Vector3 l2Dir = RotatorToL2Direction(pitch, yaw);
                var light = new Light();
                light.Type = type;
                light.Name = name;
                light.Position = ToUnityPosition(location);
                light.Direction = ToUnityDirection(l2Dir);
                light.Radius = WorldRadiusUu(radius) * UuToMeters;
                light.Color = LightPlane(hue, saturation, brightness);
                light.Cone = Mathf.Clamp(cone, 0, 255);
                lights.Add(light);
            }
        }

        return lights;
    }
}
