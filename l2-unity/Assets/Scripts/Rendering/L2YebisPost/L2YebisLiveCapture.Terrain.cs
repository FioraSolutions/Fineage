using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

public partial class L2YebisLiveCapture
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

    static void WriteProbe(string folder, RenderTexture scene, RenderTexture graded)
    {
        if (scene == null || graded == null || scene.width < 8 || scene.height < 8)
        {
            return;
        }

        int x = scene.width / 2;
        int y = scene.height / 8;
        Color32 sample = ReadOne(scene, x, y);
        Color32 stored = ReadOne(graded, x, y);
        Vector3 a = new Vector3(sample.r / 255f, sample.g / 255f, sample.b / 255f);
        Vector3 b = CaptureMatrix(a);
        Vector3 filmic = CaptureFilmic(b);
        const float bias = 0.00499999523f;
        Vector3 beforePow = filmic + (Vector3.one - filmic) * bias;
        Vector3 gradedValue = CaptureGamma(beforePow);
        int stage = _gradeStage;
        Vector3 shown = stage == 1 ? a : stage == 2 ? b : stage == 3 ? beforePow : gradedValue;
        int fileRow = scene.height - 1 - y;
        var text = new StringBuilder();
        text.AppendLine("colorSpace=" + QualitySettings.activeColorSpace);
        text.AppendLine("sceneFormat=" + scene.graphicsFormat);
        text.AppendLine("gradeFormat=" + graded.graphicsFormat);
        text.AppendLine("stage=" + stage + " " + GradeStageName(stage));
        text.AppendLine("Точка ReadPixels, ноль снизу слева: x=" + x + " y=" + y);
        text.AppendLine("Та же точка в input.png, строка 0 сверху: x=" + x + " row=" + fileRow);
        text.AppendLine("Если режим A и персонаж стоит ногами вниз, точка на полу перед ним.");
        text.AppendLine("A байты " + sample.r + " " + sample.g + " " + sample.b);
        text.AppendLine("A " + Fmt(a));
        text.AppendLine("B матрица без обрезки " + Fmt(b));
        text.AppendLine("filmic " + Fmt(filmic));
        text.AppendLine("C перед степенью, с чёрным бликом 0.005 " + Fmt(beforePow));
        text.AppendLine("D после степени 0.625 " + Fmt(gradedValue));
        text.AppendLine("RT байты выхода " + stored.r + " " + stored.g + " " + stored.b);
        text.AppendLine("Ожидание записи этой стадии, байты " + ToByte(shown.x) + " " + ToByte(shown.y) + " " + ToByte(shown.z));
        text.AppendLine("Проект Gamma, формат камеры 8-битный: второй гаммы при записи в RT нет. Байт 255 значит, что значение стадии было выше 1.");
        File.WriteAllText(Path.Combine(folder, "probe.txt"), text.ToString(), Encoding.UTF8);
        // Debug.Log("[Yebis] probe A " + Fmt(a) + " B " + Fmt(b) + " C " + Fmt(beforePow) + " D " + Fmt(gradedValue) + " rt " + stored.r + " " + stored.g + " " + stored.b);
    }

    static Color32 ReadOne(RenderTexture rt, int x, int y)
    {
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = rt;
        var tex = new Texture2D(1, 1, TextureFormat.RGBA32, false, true);
        tex.ReadPixels(new Rect(x, y, 1f, 1f), 0, 0, false);
        tex.Apply(false, false);
        RenderTexture.active = previous;
        Color32 pixel = tex.GetPixel(0, 0);
        UnityEngine.Object.Destroy(tex);
        return pixel;
    }

    static Vector3 CaptureMatrix(Vector3 scene)
    {
        const float bias = 0.00499999523f;
        const float floor = 5.96046448e-8f;
        float r = scene.x * 4.75511837f + scene.y * 0.177011937f + scene.z * 0.0178695004f + bias;
        float g = scene.x * 0.0526184812f + scene.y * 4.87951183f + scene.z * 0.0178695004f + bias;
        float b = scene.x * 0.0526184812f + scene.y * 0.177011937f + scene.z * 4.72036934f + bias;
        return new Vector3(Mathf.Max(r, floor), Mathf.Max(g, floor), Mathf.Max(b, floor));
    }

    static Vector3 CaptureFilmic(Vector3 x)
    {
        const float filmicA = 2.65814137f;
        const float filmicB = 0.664993107f;
        return new Vector3(FilmicChannel(x.x, filmicA, filmicB), FilmicChannel(x.y, filmicA, filmicB), FilmicChannel(x.z, filmicA, filmicB));
    }

    static float FilmicChannel(float x, float filmicA, float filmicB)
    {
        float e = Mathf.Exp(-filmicA * x);
        float oneMinusBe = 1f - filmicB * e;
        return Mathf.Clamp01((1f - e) * oneMinusBe * oneMinusBe);
    }

    static Vector3 CaptureGamma(Vector3 combined)
    {
        const float floor = 5.96046448e-8f;
        return new Vector3(GammaChannel(combined.x, floor), GammaChannel(combined.y, floor), GammaChannel(combined.z, floor));
    }

    static float GammaChannel(float value, float floor)
    {
        float safe = Mathf.Max(value, floor);
        return Mathf.Clamp01(Mathf.Exp(Mathf.Log(safe) * 0.625f));
    }

    static string Fmt(Vector3 value)
    {
        return value.x.ToString("0.####", CultureInfo.InvariantCulture) + " " +
               value.y.ToString("0.####", CultureInfo.InvariantCulture) + " " +
               value.z.ToString("0.####", CultureInfo.InvariantCulture);
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
