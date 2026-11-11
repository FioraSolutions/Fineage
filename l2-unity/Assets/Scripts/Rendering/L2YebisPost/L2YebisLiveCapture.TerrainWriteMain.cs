using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

public static partial class L2YebisLiveCapture
{
    static void WriteTerrainColor0(string folder, RenderTexture scene)
    {
        Camera cam = Camera.main;
        var text = new StringBuilder();
        text.AppendLine("role=unity-near-path-color0");
        text.AppendLine("Color0 треугольника под той же долей экрана, что и ближняя тропа клиента. Пиксель получает интерполяцию вершин.");
        text.AppendLine("shader_color0 повторяет вершинный шейдер: чёрный COLOR становится белым, белый обнуляется, дальше ambient, HSV и байт intensity этой точки.");
        if (cam == null)
        {
            text.AppendLine("camera=missing");
            File.WriteAllText(Path.Combine(folder, "terrain_color0.txt"), text.ToString(), Encoding.UTF8);
            return;
        }

        Vector4 origin = Shader.GetGlobalVector("_L2TerrainIntensityOrigin");
        Vector4 ambient = Shader.GetGlobalVector("_L2TerrainAmbient");
        Vector4 hsv = Shader.GetGlobalVector("_L2HsvTerrainLight");
        float fogEnd = Shader.GetGlobalFloat("_L2FogEnd");
        float fogScale = Shader.GetGlobalFloat("_L2FogScale");
        float fogEnable = Shader.GetGlobalFloat("_L2FogEnable");
        var intensityTex = Shader.GetGlobalTexture("_L2TerrainIntensityMap") as Texture2D;
        text.AppendLine("ambient " + Fmt(new Vector3(ambient.x, ambient.y, ambient.z)));
        text.AppendLine("hsv " + Fmt(new Vector3(hsv.x, hsv.y, hsv.z)));
        text.AppendLine(
            "intensity_origin " + origin.x.ToString("0.###", CultureInfo.InvariantCulture) + " " +
            origin.y.ToString("0.###", CultureInfo.InvariantCulture) + " cell " +
            origin.z.ToString("0.###", CultureInfo.InvariantCulture) + " grid " +
            origin.w.ToString("0.###", CultureInfo.InvariantCulture));
        text.AppendLine("intensity_texture=" + (intensityTex != null ? "yes" : "no"));

        var regions = new[]
        {
            new Region("near_left", 0.24f, 0.83f),
            new Region("far", 0.49f, 0.42f),
            new Region("near_right", 0.69f, 0.88f)
        };
        foreach (Region region in regions)
        {
            text.AppendLine("region=" + region.Name + " screen_from_top=" +
                region.X.ToString("0.000", CultureInfo.InvariantCulture) + "," +
                region.Y.ToString("0.000", CultureInfo.InvariantCulture));
            if (scene != null && scene.width > 1 && scene.height > 1)
            {
                int px = Mathf.Clamp(Mathf.RoundToInt(region.X * scene.width), 0, scene.width - 1);
                int pyTop = Mathf.Clamp(Mathf.RoundToInt(region.Y * scene.height), 0, scene.height - 1);
                int py = scene.height - 1 - pyTop;
                Color32 sample = ReadOne(scene, px, py);
                text.AppendLine("scene_before_curve " + sample.r + " " + sample.g + " " + sample.b +
                    " pixel " + px + "," + pyTop);
            }
            Ray ray = cam.ViewportPointToRay(new Vector3(region.X, 1f - region.Y, 0f));
            RaycastHit[] hits = Physics.RaycastAll(ray, 400f);
            RaycastHit ground = default;
            bool found = false;
            float best = float.MaxValue;
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].normal.y < 0.45f || hits[i].distance >= best)
                {
                    continue;
                }

                best = hits[i].distance;
                ground = hits[i];
                found = true;
            }

            if (!found)
            {
                text.AppendLine("hit=none");
                continue;
            }

            AppendHit(text, cam, ground, origin, ambient, hsv, intensityTex, fogEnd, fogScale, fogEnable);
        }

        File.WriteAllText(Path.Combine(folder, "terrain_color0.txt"), text.ToString(), Encoding.UTF8);
    }
}
