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
}
