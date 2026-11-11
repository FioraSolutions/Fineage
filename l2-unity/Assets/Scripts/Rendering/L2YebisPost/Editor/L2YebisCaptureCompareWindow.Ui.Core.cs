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
using UnityEngine.UIElements;

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
}
