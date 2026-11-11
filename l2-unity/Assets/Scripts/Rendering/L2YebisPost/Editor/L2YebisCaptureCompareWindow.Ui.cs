using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

public sealed partial class L2YebisCaptureCompareWindow : EditorWindow
{
    [MenuItem("L2/Yebis/Compare Capture")]
    static void Open()
    {
        var window = GetWindow<L2YebisCaptureCompareWindow>("Yebis Capture");
        window.minSize = new Vector2(760, 640);
        window._folder = EditorPrefs.GetString(PathPref, "");
    }

    void OnDisable()
    {
        DestroyPreview(ref _inputPreview);
        DestroyPreview(ref _clientPreview);
        DestroyPreview(ref _unityPreview);
        DestroyPreview(ref _errorPreview);
    }

    void OnGUI()
    {
        _scroll = EditorGUILayout.BeginScrollView(_scroll);
        EditorGUILayout.LabelField("Снимок одного draw. Сцена Unity не рисуется.", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("s0 идёт в матрицу и filmic. s1 — блик. Если s1.raw нет, блик чёрный.");

        EditorGUILayout.BeginHorizontal();
        _folder = EditorGUILayout.TextField("Папка", _folder);
        if (GUILayout.Button("Выбрать", GUILayout.Width(90)))
        {
            string picked = EditorUtility.OpenFolderPanel("Папка draw_XXX", _folder, "");
            if (!string.IsNullOrEmpty(picked))
            {
                _folder = picked;
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Последний снимок клиента"))
        {
            if (TryLatestDraw(out string latest))
            {
                _folder = latest;
            }
            else
            {
                _report = "В " + GameCaptures + " ещё нет draw с output.raw.";
            }
        }
        if (GUILayout.Button("Сравнить"))
        {
            Compare();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Снять живой вход"))
        {
            if (!EditorApplication.isPlaying)
            {
                _report = "Сначала Play Mode. Затем эта кнопка или клавиша G в окне Game. Солнечное время в Play держится на 12:00.";
            }
            else
            {
                L2YebisLiveCapture.Request();
                _report = "Кадр снимется на следующем проходе. Папка появится в YebisCapture\\unity_live.";
            }
        }
        if (GUILayout.Button("Последний живой снимок"))
        {
            if (TryLatestUnity(out string live))
            {
                _folder = live;
            }
            else
            {
                _report = "Живого снимка ещё нет. Play Mode, затем G или «Снять живой вход».";
            }
        }
        if (GUILayout.Button("Свести входы"))
        {
            CompareInputs();
        }
        if (GUILayout.Button("Сверить кривую"))
        {
            CompareCurve();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.HelpBox(_report, MessageType.None);
        DrawPair("Вход", _inputPreview, "Клиент после draw", _clientPreview);
        DrawPair("Unity", _unityPreview, _errorLabel, _errorPreview);
        EditorGUILayout.EndScrollView();
    }

    static void DrawPair(string leftLabel, Texture left, string rightLabel, Texture right)
    {
        EditorGUILayout.BeginHorizontal();
        DrawPreview(leftLabel, left);
        DrawPreview(rightLabel, right);
        EditorGUILayout.EndHorizontal();
    }

    static void DrawPreview(string label, Texture texture)
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(360));
        EditorGUILayout.LabelField(label);
        Rect rect = GUILayoutUtility.GetRect(340, 190, GUILayout.ExpandWidth(true));
        if (texture != null)
        {
            EditorGUI.DrawPreviewTexture(rect, texture, null, ScaleMode.ScaleToFit);
        }
        EditorGUILayout.EndVertical();
    }

    void Compare()
    {
        string folder = ResolveDrawFolder(_folder);
        if (string.IsNullOrEmpty(folder))
        {
            _report = "Нет output.raw. Выбери папку draw_XXX или корень снимка.";
            return;
        }

        _folder = folder;
        EditorPrefs.SetString(PathPref, folder);

        try
        {
            EditorUtility.DisplayProgressBar("Yebis capture", "Чтение входа", 0.2f);
            Dictionary<string, string> inputMeta = ReadMeta(Path.Combine(folder, "input.meta.txt"));
            Dictionary<string, string> outputMeta = ReadMeta(Path.Combine(folder, "output.meta.txt"));
            Dictionary<string, string> constants = ReadConstants(Path.Combine(folder, "constants.txt"));
            Dictionary<string, string> gpu = ReadMeta(Path.Combine(folder, "gpu-state.txt"));

            float[] input = LoadRaw(Path.Combine(folder, "input.raw"), inputMeta, out int width, out int height);
            EditorUtility.DisplayProgressBar("Yebis capture", "Чтение выхода клиента", 0.4f);
            float[] client = LoadRaw(Path.Combine(folder, "output.raw"), outputMeta, out int outWidth, out int outHeight);

            bool hasGlare = TryLoadOptional(folder, "s1", out float[] glare, out int glareWidth, out int glareHeight);
            bool hasDither = TryLoadOptional(folder, "s3", out float[] dither, out int ditherWidth, out int ditherHeight);
            int compareWidth = Math.Min(width, outWidth);
            int compareHeight = Math.Min(height, outHeight);
            bool quantize8 = Value(outputMeta, "memoryOrder") == "B8G8R8A8" || Value(outputMeta, "memoryOrder") == "R8G8B8A8";

            EditorUtility.DisplayProgressBar("Yebis capture", "Прогон формулы", 0.7f);
            float[] unity = RunGrade(input, width, height, glare, glareWidth, glareHeight, hasGlare, dither, ditherWidth, ditherHeight, hasDither, constants, gpu);
            EditorUtility.DisplayProgressBar("Yebis capture", "Разница", 0.9f);

            var stats = new StringBuilder();
            stats.AppendLine(hasDither ? "С дизером, уровни байта 0..255:" : "Уровни байта 0..255:");
            float[] error = BuildError(unity, width, client, outWidth, compareWidth, compareHeight, stats);
            if (hasDither)
            {
                float[] unityOff = RunGrade(input, width, height, glare, glareWidth, glareHeight, hasGlare, dither, ditherWidth, ditherHeight, false, constants, gpu);
                var off = new StringBuilder();
                BuildError(unityOff, width, client, outWidth, compareWidth, compareHeight, off);
                stats.AppendLine("Без дизера, уровни байта 0..255:");
                stats.Append(off);
            }
            AppendUvNote(stats, folder);
            AppendSourceBands(stats, "Вход s0", input, width, height);
            float[] shown = error;
            _errorLabel = "Пиксели с разницей от 1 уровня";
            if (hasGlare)
            {
                float[] blackGlare = RunGrade(input, width, height, glare, glareWidth, glareHeight, false, dither, ditherWidth, ditherHeight, hasDither, constants, gpu);
                shown = BuildPairDelta(unity, blackGlare, width, height, stats);
                _errorLabel = "Вклад s1: серое 1 уровень, белое 2+";
            }

            DestroyPreview(ref _inputPreview);
            DestroyPreview(ref _clientPreview);
            DestroyPreview(ref _unityPreview);
            DestroyPreview(ref _errorPreview);
            _inputPreview = MakeTexture(input, width, height);
            _clientPreview = MakeTexture(client, outWidth, outHeight);
            _unityPreview = MakeTexture(unity, width, height);
            int shownWidth = hasGlare ? width : compareWidth;
            int shownHeight = hasGlare ? height : compareHeight;
            _errorPreview = MakeTexture(shown, shownWidth, shownHeight);

            var header = new StringBuilder();
            header.AppendLine(folder);
            header.AppendLine("Вход " + width + "x" + height + " " + Value(inputMeta, "formatName") + " " + Value(inputMeta, "memoryOrder"));
            header.AppendLine("Выход " + outWidth + "x" + outHeight + " " + Value(outputMeta, "formatName") + " " + Value(outputMeta, "memoryOrder"));
            if (width != outWidth || height != outHeight)
            {
                header.AppendLine("Размеры различаются. Сравнение идёт по общей области " + compareWidth + "x" + compareHeight + ".");
            }
            AppendConstant(header, constants, "c4");
            AppendConstant(header, constants, "c12");
            AppendConstant(header, constants, "c14");
            header.AppendLine(hasGlare
                ? "s1 загружен, " + glareWidth + "x" + glareHeight + "."
                : "s1.raw нет. Блик чёрный, в формуле остаётся смещение 0.005 второй матрицы.");
            header.AppendLine(hasDither
                ? "s3 загружен, " + ditherWidth + "x" + ditherHeight + ". Дизер включён."
                : "s3.raw нет. Дизер выключен, сравнивается ветка до него.");
            header.AppendLine("Pre3898, экспозиция и SatMix в этот прогон не входят.");
            if (quantize8)
            {
                header.AppendLine("Выход клиента 8-битный. Разница считается в целых уровнях 0..255.");
            }
            if (Value(gpu, "srgbWrite") == "1" || Value(gpu, "sampler0Srgb") == "1")
            {
                header.AppendLine("В снимке включён sRGB write или sampler. Сырые числа загружены без дополнительной гаммы.");
            }
            header.Append(stats);
            _report = header.ToString();
            File.WriteAllText(Path.Combine(folder, "unity_compare.txt"), _report, Encoding.UTF8);
        }
        catch (Exception ex)
        {
            _report = ex.Message;
        }
        finally
        {
            EditorUtility.ClearProgressBar();
            Repaint();
        }
    }

    float[] RunGrade(
        float[] input,
        int width,
        int height,
        float[] glare,
        int glareWidth,
        int glareHeight,
        bool hasGlare,
        float[] dither,
        int ditherWidth,
        int ditherHeight,
        bool hasDither,
        Dictionary<string, string> constants,
        Dictionary<string, string> gpu)
    {
        Shader shader = Shader.Find("Hidden/L2/YebisCaptureReplay");
        if (shader == null)
        {
            throw new InvalidOperationException("Не найден шейдер Hidden/L2/YebisCaptureReplay.");
        }

        Texture2D source = MakeTexture(input, width, height);
        Texture2D glareTex = hasGlare ? MakeTexture(glare, glareWidth, glareHeight) : BlackTex();
        Texture2D ditherTex = hasDither ? MakeTexture(dither, ditherWidth, ditherHeight) : BlackTex();
        source.filterMode = SamplerFilter(gpu, "sampler0Mag");
        glareTex.filterMode = SamplerFilter(gpu, "sampler1Mag");
        ditherTex.filterMode = SamplerFilter(gpu, "sampler3Mag");
        source.wrapMode = SamplerWrap(gpu, "sampler0AddressU");
        glareTex.wrapMode = SamplerWrap(gpu, "sampler1AddressU");
        ditherTex.wrapMode = SamplerWrap(gpu, "sampler3AddressU");
        var material = new Material(shader) { hideFlags = HideFlags.HideAndDontSave };
        BindGrade(material, constants, hasDither);
        material.SetTexture("_GlareTex", glareTex);
        material.SetTexture("_DitherTex", ditherTex);

        var desc = new RenderTextureDescriptor(width, height, GraphicsFormat.R32G32B32A32_SFloat, 0)
        {
            sRGB = false,
            mipCount = 1,
            msaaSamples = 1,
            useMipMap = false
        };
        var rt = new RenderTexture(desc)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp,
            hideFlags = HideFlags.HideAndDontSave
        };
        rt.Create();

        try
        {
            Graphics.Blit(source, rt, material);
            AsyncGPUReadbackRequest request = AsyncGPUReadback.Request(rt, 0, GraphicsFormat.R32G32B32A32_SFloat);
            request.WaitForCompletion();
            if (request.hasError)
            {
                throw new InvalidOperationException("GPU не отдал результат прогона.");
            }

            NativeArray<float> raw = request.GetData<float>();
            var pixels = new float[raw.Length];
            raw.CopyTo(pixels);
            return pixels;
        }
        finally
        {
            DestroyImmediate(source);
            DestroyImmediate(glareTex);
            DestroyImmediate(ditherTex);
            DestroyImmediate(material);
            rt.Release();
            DestroyImmediate(rt);
        }
    }

    void BindGrade(Material material, Dictionary<string, string> constants, bool hasDither)
    {
        material.SetVector("_CapC4", VectorOf(constants, "c4", new Vector4(4.7551184f, 0.17701194f, 0.0178695f, 0.005f)));
        material.SetVector("_CapC5", VectorOf(constants, "c5", new Vector4(0.052618481f, 4.8795118f, 0.0178695f, 0.005f)));
        material.SetVector("_CapC6", VectorOf(constants, "c6", new Vector4(0.052618481f, 0.17701194f, 4.7203693f, 0.005f)));
        material.SetVector("_CapC7", VectorOf(constants, "c7", new Vector4(0.9510237f, 0.035402387f, 0.0035739001f, 0.005f)));
        material.SetVector("_CapC8", VectorOf(constants, "c8", new Vector4(0.010523696f, 0.97590238f, 0.0035739001f, 0.005f)));
        material.SetVector("_CapC9", VectorOf(constants, "c9", new Vector4(0.010523696f, 0.035402387f, 0.94407392f, 0.005f)));
        Vector4 filmic = VectorOf(constants, "c14", new Vector4(2.6581414f, 0.66499311f, 0f, 0f));
        material.SetFloat("_FilmicA", filmic.x);
        material.SetFloat("_FilmicB", filmic.y);
        Vector4 gamma = VectorOf(constants, "c12", new Vector4(0.625f, 0f, 0f, 0f));
        material.SetFloat("_GammaPow", gamma.x);
        material.SetVector("_Dither", VectorOf(constants, "c13", new Vector4(1f / 255f, 1f / 255f, -1f / 255f, 0f)));
        material.SetFloat("_HasDither", hasDither ? 1f : 0f);
    }

    static Texture2D BlackTex()
    {
        var tex = MakeTexture(new float[] { 0f, 0f, 0f, 1f }, 1, 1);
        return tex;
    }

    static FilterMode SamplerFilter(Dictionary<string, string> gpu, string key)
    {
        return Value(gpu, key) == "2" ? FilterMode.Bilinear : FilterMode.Point;
    }

    static TextureWrapMode SamplerWrap(Dictionary<string, string> gpu, string key)
    {
        string mode = Value(gpu, key);
        if (mode == "1")
        {
            return TextureWrapMode.Repeat;
        }
        if (mode == "2")
        {
            return TextureWrapMode.Mirror;
        }
        return TextureWrapMode.Clamp;
    }

    static bool TryLoadOptional(string folder, string name, out float[] pixels, out int width, out int height)
    {
        pixels = null;
        width = 0;
        height = 0;
        string raw = Path.Combine(folder, name + ".raw");
        string metaPath = Path.Combine(folder, name + ".meta.txt");
        if (!File.Exists(raw) || !File.Exists(metaPath))
        {
            return false;
        }
        Dictionary<string, string> meta = ReadMeta(metaPath);
        if (Value(meta, "present") == "0")
        {
            return false;
        }
        pixels = LoadRaw(raw, meta, out width, out height);
        return true;
    }

    static float[] BuildError(
        float[] unity,
        int unityWidth,
        float[] client,
        int clientWidth,
        int width,
        int height,
        StringBuilder stats)
    {
        var error = new float[width * height * 4];
        int count = width * height;
        int maxLevel = 0;
        int pixels0 = 0, pixels1 = 0, pixels2 = 0, pixelsMore = 0;
        int channels0 = 0, channels1 = 0, channels2 = 0, channelsMore = 0;
        int marked = 0, step1 = 0, step4x = 0, step4y = 0;
        var mark = new bool[count];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int u = (y * unityWidth + x) * 4;
                int c = (y * clientWidth + x) * 4;
                int e = (y * width + x) * 4;
                int dr = ChannelDelta(unity[u], client[c]);
                int dg = ChannelDelta(unity[u + 1], client[c + 1]);
                int db = ChannelDelta(unity[u + 2], client[c + 2]);
                CountChannel(dr, ref channels0, ref channels1, ref channels2, ref channelsMore);
                CountChannel(dg, ref channels0, ref channels1, ref channels2, ref channelsMore);
                CountChannel(db, ref channels0, ref channels1, ref channels2, ref channelsMore);
                int worst = Math.Max(dr, Math.Max(dg, db));
                maxLevel = Math.Max(maxLevel, worst);
                if (worst == 0) pixels0++;
                else if (worst == 1) pixels1++;
                else if (worst == 2) pixels2++;
                else pixelsMore++;
                bool hit = worst >= 1;
                mark[y * width + x] = hit;
                if (hit) marked++;
                float shown = hit ? 1f : 0f;
                error[e] = shown;
                error[e + 1] = shown;
                error[e + 2] = shown;
                error[e + 3] = 1f;
            }
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (!mark[y * width + x]) continue;
                if (x + 1 < width && mark[y * width + x + 1]) step1++;
                if (x + 4 < width && mark[y * width + x + 4]) step4x++;
                if (y + 4 < height && mark[(y + 4) * width + x]) step4y++;
            }
        }

        stats.AppendLine("максимум уровней: " + maxLevel);
        stats.AppendLine("пиксели  0: " + pixels0 + "   1: " + pixels1 + "   2: " + pixels2 + "   3+: " + pixelsMore + "   из " + count);
        stats.AppendLine("каналы   0: " + channels0 + "   1: " + channels1 + "   2: " + channels2 + "   3+: " + channelsMore);
        if (marked > 0)
        {
            stats.AppendLine(
                "среди пикселей с разницей сосед через 1 px тоже с разницей: " + step1 +
                " из " + marked + "; через 4 px по X: " + step4x + "; через 4 px по Y: " + step4y);
        }
        return error;
    }
}
