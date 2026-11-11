using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

public static partial class L2ActorLightCapture
{
    static string Vec(Color c)
    {
        return Num(c.r) + " " + Num(c.g) + " " + Num(c.b) + " " + Num(c.a);
    }

    static string Vec(Vector3 v)
    {
        return Num(v.x) + " " + Num(v.y) + " " + Num(v.z) + " 0";
    }

    static string Vec(Vector4 v)
    {
        return Num(v.x) + " " + Num(v.y) + " " + Num(v.z) + " " + Num(v.w);
    }

    public static void Write()
    {
        float hours = WorldClockPacketServices.Clock != null ? WorldClockPacketServices.Clock.WorldHours : -1f;
        L2ActorLightLut.EnsureLoaded();
        L2CelestialLut.EnsureLoaded();

        Color half = Shader.GetGlobalColor("_L2ActorAmbientHalf");
        Color plane = Shader.GetGlobalColor("_L2HsvActorLight");
        Color basis = Shader.GetGlobalColor("_L2HsvActorBase");
        Color actor = Shader.GetGlobalColor("_L2ActorAmbient");
        Vector4 lightDir = Shader.GetGlobalVector("_L2StaticMeshLightDirection");
        Color fog = Shader.GetGlobalColor("_L2FogColor");
        float fogEnd = Shader.GetGlobalFloat("_L2FogEnd");
        float fogScale = Shader.GetGlobalFloat("_L2FogScale");
        float fogEnable = Shader.GetGlobalFloat("_L2FogEnable");
        float sunScale = Shader.GetGlobalFloat("_L2ActorDirectionalScale");
        if (sunScale <= 0f)
            sunScale = L2ActorLightLut.DirectionalScale;

        Vector3 sun = lightDir.sqrMagnitude > 1e-8f ? ((Vector3)lightDir).normalized : Vector3.zero;
        Vector3 c249L2 = new Vector3(sun.z, sun.x, sun.y);
        Color c250 = new Color(plane.r * sunScale, plane.g * sunScale, plane.b * sunScale, 1f);
        bool linear = QualitySettings.activeColorSpace == ColorSpace.Linear;

        string folder = Path.Combine(Root, DateTime.Now.ToString("yyyyMMdd_HHmmss"));
        Directory.CreateDirectory(folder);

        var text = new StringBuilder();
        text.AppendLine("role=unity-actor");
        text.AppendLine("worldHours=" + Num(hours));
        text.AppendLine("colorSpace=" + QualitySettings.activeColorSpace);
        text.AppendLine("sunScale=" + Num(sunScale));
        text.AppendLine("note=c249_l2 is _L2StaticMeshLightDirection in L2 axes (X, Y, Z-up). The shader uses saturate(-dot(N, that Unity vector)).");
        text.AppendLine("note=c250 is plane * sunScale. plane is _L2HsvActorLight.rgb. Its alpha is brightness/255 and is not multiplied.");
        text.AppendLine("note=client_* is monster_008 at t=16.976. Color delta is meaningful at any hour only for the shape of the curve; direction delta is for that hour.");
        if (linear)
        {
            text.AppendLine("note=Linear project. GetGlobalColor below is the value that was set. The shader receives the sRGB-to-linear conversion of these colors.");
        }

        text.AppendLine("c251=" + Vec(half));
        text.AppendLine("c250=" + Vec(c250));
        text.AppendLine("c249_l2=" + Vec(c249L2));
        text.AppendLine("c249_unity=" + Vec(sun));
        text.AppendLine("plane=" + Vec(plane));
        text.AppendLine("brightness01=" + Num(plane.a));
        text.AppendLine("base=" + Vec(basis));
        text.AppendLine("actorAmbient=" + Vec(actor));
        text.AppendLine("fogColor=" + Vec(fog) + " bytes=" + Byte(fog.r) + " " + Byte(fog.g) + " " + Byte(fog.b));
        text.AppendLine("fogEnd=" + Num(fogEnd));
        text.AppendLine("fogScale=" + Num(fogScale));
        text.AppendLine("fogEnable=" + Num(fogEnable));
        text.AppendLine("fogStartUu=4096");
        text.AppendLine("fogEndUu=12288");
        text.AppendLine("alphaTest=1");
        text.AppendLine("alphaRef=127");
        text.AppendLine("cull=none");
        text.AppendLine("colorOp=MODULATE2X");
        text.AppendLine("shadowMul=" + Vec(half * 2f));
        text.AppendLine("litMul=" + Vec(new Color(
            Mathf.Min((half.r + c250.r) * 2f, 1f),
            Mathf.Min((half.g + c250.g) * 2f, 1f),
            Mathf.Min((half.b + c250.b) * 2f, 1f),
            1f)));

        if (linear)
        {
            text.AppendLine("c251_shader=" + Vec(ToLinear(half)));
            text.AppendLine("c250_shader=" + Vec(ToLinear(c250)));
        }

        text.AppendLine("client_c251=" + Vec(ClientC251));
        text.AppendLine("client_c250=" + Vec(ClientC250));
        text.AppendLine("client_c249=" + Vec(ClientC249));
        text.AppendLine("delta_c251=" + Vec(new Vector3(half.r - ClientC251.x, half.g - ClientC251.y, half.b - ClientC251.z)));
        text.AppendLine("delta_c250=" + Vec(new Vector3(c250.r - ClientC250.x, c250.g - ClientC250.y, c250.b - ClientC250.z)));
        text.AppendLine("delta_c249_l2=" + Vec(c249L2 - (Vector3)ClientC249));

        if (L2ActorLightLut.IsReady && hours >= 0f)
        {
            L2ActorLightLut.Sample sample = L2ActorLightLut.SampleAt(hours);
            text.AppendLine("lut_half=" + Vec(sample.ambientHalf));
            text.AppendLine("lut_plane=" + Vec(sample.sunPlane));
            text.AppendLine("lut_base=" + Vec(sample.sunBase));
        }

        if (L2CelestialLut.IsReady && hours >= 0f)
        {
            Vector3 sunDir = L2CelestialLut.SampleAt(hours).sunDirUnity;
            text.AppendLine("celestial_sun_unity=" + Vec(sunDir));
        }

        text.AppendLine("---- point lights ----");
        L2ActorPointLights.AppendCapture(text);

        text.AppendLine("---- katerina ----");
        string katerina = AppendKaterina(text);

        string path = Path.Combine(folder, "constants.txt");
        File.WriteAllText(path, text.ToString(), Encoding.UTF8);
        File.WriteAllText(Path.Combine(Root, "latest.txt"), folder, Encoding.UTF8);
        // Debug.Log("[ActorLight] запись сделана " + path + " t=" + Num(hours) +
            // " c251=" + Num(half.r) + " c250=" + Num(c250.r) + " " + katerina);
    }
}
