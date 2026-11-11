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

    static void AppendHit(
        StringBuilder text,
        Camera cam,
        RaycastHit ground,
        Vector4 origin,
        Vector4 ambient,
        Vector4 hsv,
        Texture2D intensityTex,
        float fogEnd,
        float fogScale,
        float fogEnable)
    {
        Vector3 amb = new Vector3(ambient.x, ambient.y, ambient.z);
        Vector3 plane = new Vector3(hsv.x, hsv.y, hsv.z);
        float viewAbsZ = Mathf.Abs(cam.worldToCameraMatrix.MultiplyPoint(ground.point).z);
        float fogF = fogEnable > 0.5f ? Mathf.Clamp01((fogEnd - viewAbsZ) * fogScale) : 1f;
        text.AppendLine("hit_world " + Fmt(ground.point) + " viewAbsZ " + viewAbsZ.ToString("0.###", CultureInfo.InvariantCulture) +
            " fog_f " + fogF.ToString("0.###", CultureInfo.InvariantCulture));
        AppendPoint(text, "hit", ground.point, Vector3.zero, origin, amb, plane, intensityTex);

        MeshCollider meshCollider = ground.collider as MeshCollider;
        Mesh mesh = meshCollider != null ? meshCollider.sharedMesh : null;
        if (mesh == null || ground.triangleIndex < 0)
        {
            text.AppendLine("triangle=unavailable");
            return;
        }

        int[] tris;
        try
        {
            tris = mesh.triangles;
        }
        catch (Exception)
        {
            text.AppendLine("triangle=unreadable");
            return;
        }
        int tri = ground.triangleIndex * 3;
        if (tri + 2 >= tris.Length)
        {
            text.AppendLine("triangle=out_of_range");
            return;
        }

        Vector3[] vertices;
        Color32[] colors = null;
        try
        {
            vertices = mesh.vertices;
            if (mesh.HasVertexAttribute(VertexAttribute.Color))
            {
                colors = mesh.colors32;
            }
        }
        catch (Exception)
        {
            text.AppendLine("triangle=unreadable");
            return;
        }
        Vector3 bary = ground.barycentricCoordinate;
        Vector3 interpolated = Vector3.zero;
        for (int v = 0; v < 3; v++)
        {
            int index = tris[tri + v];
            Vector3 world = meshCollider.transform.TransformPoint(vertices[index]);
            Vector3 meshColor = Vector3.zero;
            if (colors != null && index < colors.Length)
            {
                meshColor = new Vector3(colors[index].r / 255f, colors[index].g / 255f, colors[index].b / 255f);
            }

            Vector3 color0 = TerrainShaderColor0(meshColor, amb, plane, Intensity01(world, origin, intensityTex));
            interpolated += color0 * bary[v];
            text.AppendLine(
                "v" + v + " world " + Fmt(world) +
                " mesh " + (colors != null && index < colors.Length
                    ? colors[index].r + " " + colors[index].g + " " + colors[index].b
                    : "none") +
                " shader_color0 " + Fmt(color0));
            AppendPoint(text, "v" + v, world, meshColor, origin, amb, plane, intensityTex);
        }

        text.AppendLine("pixel_color0 " + Fmt(interpolated));
    }

    static void AppendPoint(
        StringBuilder text,
        string tag,
        Vector3 world,
        Vector3 meshColor,
        Vector4 origin,
        Vector3 ambient,
        Vector3 hsv,
        Texture2D intensityTex)
    {
        float cell = Mathf.Max(origin.z, 1f);
        float grid = Mathf.Max(origin.w, 1f);
        float worldUuX = world.x * 52.5f;
        float worldUuY = world.z * 52.5f;
        float gx = (worldUuY - origin.x) / cell;
        float gy = (worldUuX - origin.y) / cell;
        int ix = Mathf.FloorToInt(gx);
        int iy = Mathf.FloorToInt(gy);
        int sectorX = ix >= 0 ? (ix / 16) * 16 : -(((-ix) + 15) / 16) * 16;
        int sectorY = iy >= 0 ? (iy / 16) * 16 : -(((-iy) + 15) / 16) * 16;
        int localX = ix - sectorX;
        int localY = iy - sectorY;
        int intensity = -1;
        int valid = 0;
        if (intensityTex != null && ix >= 0 && iy >= 0 && ix < intensityTex.width && iy < intensityTex.height)
        {
            Color32 pixel = intensityTex.GetPixel(ix, iy);
            intensity = pixel.r;
            valid = pixel.g;
        }

        float intensity01 = intensity >= 0 ? intensity / 255f : Shader.GetGlobalFloat("_L2TerrainIntensity");
        Vector3 color0 = TerrainShaderColor0(meshColor, ambient, hsv, intensity01);
        text.AppendLine(
            tag + "_cell gx=" + ix + " gy=" + iy +
            " sector=" + sectorX + "," + sectorY +
            " local=" + localX + "," + localY +
            " intensity_byte=" + intensity +
            " valid=" + valid +
            " color0 " + Fmt(color0));
    }

    static float Intensity01(Vector3 world, Vector4 origin, Texture2D intensityTex)
    {
        float cell = Mathf.Max(origin.z, 1f);
        float worldUuX = world.x * 52.5f;
        float worldUuY = world.z * 52.5f;
        int ix = Mathf.FloorToInt((worldUuY - origin.x) / cell);
        int iy = Mathf.FloorToInt((worldUuX - origin.y) / cell);
        if (intensityTex != null && ix >= 0 && iy >= 0 && ix < intensityTex.width && iy < intensityTex.height)
        {
            return intensityTex.GetPixel(ix, iy).r;
        }

        return Shader.GetGlobalFloat("_L2TerrainIntensity");
    }

    static Vector3 TerrainShaderColor0(Vector3 meshColor, Vector3 ambient, Vector3 hsv, float intensity01)
    {
        Vector3 vtx = meshColor;
        if (Mathf.Max(vtx.x, Mathf.Max(vtx.y, vtx.z)) < 0.001f)
        {
            vtx = Vector3.one;
        }

        float along = Mathf.Clamp01(intensity01);
        Vector3 sunByte = new Vector3(
            Mathf.Floor(hsv.x * 0.5f * along * 255f),
            Mathf.Floor(hsv.y * 0.5f * along * 255f),
            Mathf.Floor(hsv.z * 0.5f * along * 255f));
        Vector3 ambByte = new Vector3(
            Mathf.Floor(Mathf.Clamp01(ambient.x) * 255f + 0.5f),
            Mathf.Floor(Mathf.Clamp01(ambient.y) * 255f + 0.5f),
            Mathf.Floor(Mathf.Clamp01(ambient.z) * 255f + 0.5f));
        Vector3 ambShift = new Vector3(
            Mathf.Floor(ambByte.x * 0.5f),
            Mathf.Floor(ambByte.y * 0.5f),
            Mathf.Floor(ambByte.z * 0.5f));
        if (Mathf.Min(vtx.x, Mathf.Min(vtx.y, vtx.z)) > 0.98f)
        {
            vtx = Vector3.zero;
        }

        Vector3 baseByte = new Vector3(
            Mathf.Floor(Mathf.Clamp01(vtx.x) * 255f + 0.5f),
            Mathf.Floor(Mathf.Clamp01(vtx.y) * 255f + 0.5f),
            Mathf.Floor(Mathf.Clamp01(vtx.z) * 255f + 0.5f));
        float scale = Shader.GetGlobalFloat("_L2TerrainColor0Scale");
        if (scale < 0.001f)
        {
            scale = 1f;
        }

        Vector3 color0 = Vector3.Min(baseByte + sunByte + ambShift, Vector3.one * 255f) * (scale / 255f);
        return Vector3.Min(Vector3.Max(color0, Vector3.zero), Vector3.one);
    }

    struct Region
    {
        public readonly string Name;
        public readonly float X;
        public readonly float Y;

        public Region(string name, float x, float y)
        {
            Name = name;
            X = x;
            Y = y;
        }
    }
}
