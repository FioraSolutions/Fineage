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
    const string GameCaptures =
        @"C:\games\Lineage II HighElfes\ADEU-P464-D20240703-P-240117-240724-1\system\logs\YebisCapture";

    const string PathPref = "L2.YebisCapture.Path";

    string _folder = "";
    string _report = "Папка draw_XXX из logs\\YebisCapture. Формула: s0, матрица, filmic, блик s1, гамма.";
    Vector2 _scroll;

    Texture2D _inputPreview;
    Texture2D _clientPreview;
    Texture2D _unityPreview;
    Texture2D _errorPreview;
    string _errorLabel = "Пиксели с разницей от 1 уровня";


    static Dictionary<string, string> ReadMeta(string path)
    {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (!File.Exists(path))
        {
            return map;
        }
        foreach (string raw in File.ReadAllLines(path))
        {
            int eq = raw.IndexOf('=');
            if (eq <= 0)
            {
                continue;
            }
            map[raw.Substring(0, eq).Trim()] = raw.Substring(eq + 1).Trim();
        }
        return map;
    }

    static Dictionary<string, string> ReadConstants(string path)
    {
        return ReadMeta(path);
    }
}
