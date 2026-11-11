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
    static void AppendConstant(StringBuilder text, Dictionary<string, string> constants, string key)
    {
        string value = Value(constants, key);
        if (!string.IsNullOrEmpty(value))
        {
            text.AppendLine(key + " = " + value);
        }
    }

    static string ResolveDrawFolder(string path)
    {
        if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
        {
            return null;
        }
        if (File.Exists(Path.Combine(path, "output.raw")))
        {
            return path;
        }

        string best = null;
        foreach (string child in Directory.GetDirectories(path))
        {
            if (!File.Exists(Path.Combine(child, "output.raw")))
            {
                continue;
            }
            string statePath = Path.Combine(child, "state.txt");
            if (File.Exists(statePath) && File.ReadAllText(statePath).Contains("combine=1"))
            {
                return child;
            }
            if (best == null)
            {
                best = child;
            }
        }
        return best;
    }

    static bool TryLatestDraw(out string folder)
    {
        folder = null;
        if (!Directory.Exists(GameCaptures))
        {
            return false;
        }
        string[] stamps = Directory.GetDirectories(GameCaptures);
        Array.Sort(stamps, StringComparer.Ordinal);
        for (int i = stamps.Length - 1; i >= 0; i--)
        {
            if (string.Equals(Path.GetFileName(stamps[i]), "unity_live", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }
            string draw = ResolveDrawFolder(stamps[i]);
            if (!string.IsNullOrEmpty(draw))
            {
                folder = draw;
                return true;
            }
        }
        return false;
    }

    static bool TryLatestUnity(out string folder)
    {
        folder = null;
        string root = Path.Combine(GameCaptures, "unity_live");
        string pointer = Path.Combine(root, "latest.txt");
        if (File.Exists(pointer))
        {
            string pointed = File.ReadAllText(pointer).Trim();
            if (File.Exists(Path.Combine(pointed, "input.raw")))
            {
                folder = pointed;
                return true;
            }
        }
        if (!Directory.Exists(root))
        {
            return false;
        }
        string[] stamps = Directory.GetDirectories(root);
        Array.Sort(stamps, StringComparer.Ordinal);
        for (int i = stamps.Length - 1; i >= 0; i--)
        {
            if (File.Exists(Path.Combine(stamps[i], "input.raw")))
            {
                folder = stamps[i];
                return true;
            }
        }
        return false;
    }

    void CompareInputs()
    {
        if (!TryLatestDraw(out string clientFolder) || !TryLatestUnity(out string unityFolder))
        {
            _report = "Нужны оба снимка: draw клиента с output.raw и живой снимок Unity (клавиша G в Play Mode).";
            return;
        }

        var text = new StringBuilder();
        text.AppendLine("Клиент " + clientFolder);
        text.AppendLine("Unity " + unityFolder);
        text.AppendLine("Полосы — доли кадра, не одни и те же поверхности. Камеры могут стоять по-разному.");
        AppendFolderBands(text, "Клиент s0", clientFolder, "input");
        AppendFolderBands(text, "Unity s0", unityFolder, "input");
        AppendFolderBands(text, "Клиент после combine", clientFolder, "output");
        AppendFolderBands(text, "Unity после combine", unityFolder, "output");
        _report = text.ToString();
        _folder = unityFolder;
        Repaint();
    }

    void CompareCurve()
    {
        if (!TrySceneToFloat(out string folder))
        {
            _report = "Нет draw с role=scene-to-float. Это отдельный проход перед combine.";
            return;
        }

        Dictionary<string, string> inputMeta = ReadMeta(Path.Combine(folder, "input.meta.txt"));
        Dictionary<string, string> outputMeta = ReadMeta(Path.Combine(folder, "output.meta.txt"));
        float[] input = LoadRaw(Path.Combine(folder, "input.raw"), inputMeta, out int width, out int height);
        float[] output = LoadRaw(Path.Combine(folder, "output.raw"), outputMeta, out int outWidth, out int outHeight);
        int compareWidth = Math.Min(width, outWidth);
        int compareHeight = Math.Min(height, outHeight);
        double maxAbs = 0;
        double sum = 0;
        int count = 0;
        int over = 0;
        for (int y = 0; y < compareHeight; y++)
        {
            for (int x = 0; x < compareWidth; x++)
            {
                int iIn = (y * width + x) * 4;
                int iOut = (y * outWidth + x) * 4;
                for (int c = 0; c < 3; c++)
                {
                    float got = SceneToFloat(input[iIn + c]);
                    float expected = output[iOut + c];
                    double abs = Math.Abs(got - expected);
                    if (abs > maxAbs)
                    {
                        maxAbs = abs;
                    }
                    if (abs > 0.0005)
                    {
                        over++;
                    }
                    sum += abs;
                    count++;
                }
            }
        }

        var text = new StringBuilder();
        text.AppendLine(folder);
        text.AppendLine("Кривая 0A08B2C83DB6048D на сохранённом 8-битном входе, сверка с half-float выходом.");
        text.AppendLine("Каналов " + count + "  max abs " + maxAbs.ToString("0.000000") + "  среднее " + (count > 0 ? sum / count : 0).ToString("0.000000"));
        text.AppendLine("Каналов с |разницей| > 0.0005: " + over);
        _report = text.ToString();
        _folder = folder;
        Repaint();
    }
}
