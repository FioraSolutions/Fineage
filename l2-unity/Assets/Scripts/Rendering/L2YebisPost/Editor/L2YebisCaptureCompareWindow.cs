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

}
