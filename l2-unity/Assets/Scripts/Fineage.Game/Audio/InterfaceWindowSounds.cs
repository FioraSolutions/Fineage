using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Official open/close tokens for named UI windows, from nwindow.dll InterfaceSound / ItemSound names.
/// Windows without a dedicated official wave use ItemSound.window_open / window_close.
/// </summary>
public static class InterfaceWindowSounds
{
    public const string RelativePath = "Data/Meta/Sounds/interface-window-sounds.tsv";
    public const string DefaultWindow = "*";

    public static Func<string> TextOverride;
    public static Action<string> ErrorLogged;

    static readonly Dictionary<string, (string open, string close)> ByWindow =
        new Dictionary<string, (string, string)>(StringComparer.OrdinalIgnoreCase);
    static bool _loaded;
    static string _loadError;

    public static string FormatMissingTableError(string path) =>
        "InterfaceWindowSounds: missing window sound table path=\"" + path + "\"";

    public static string FormatUnknownWindowError(string window) =>
        "InterfaceWindowSounds: no row for window=\"" + window + "\" and no default * row.";

    public static void ClearForTests()
    {
        ByWindow.Clear();
        _loaded = false;
        _loadError = null;
        TextOverride = null;
        ErrorLogged = null;
    }

    public static void Load()
    {
        if (_loaded)
            return;
        LoadFrom(ResolveDefaultText(), DefaultPath());
    }

    public static void LoadFrom(string text, string pathForErrors)
    {
        ByWindow.Clear();
        _loadError = null;
        _loaded = true;
        if (string.IsNullOrEmpty(text))
        {
            _loadError = FormatMissingTableError(pathForErrors);
            Log(_loadError);
            return;
        }

        using (var reader = new StringReader(text))
        {
            string header = reader.ReadLine();
            if (header == null)
            {
                _loadError = FormatMissingTableError(pathForErrors);
                Log(_loadError);
                return;
            }

            string line;
            int lineNo = 1;
            while ((line = reader.ReadLine()) != null)
            {
                lineNo++;
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    continue;
                string[] cols = line.Split('\t');
                if (cols.Length < 3 || string.IsNullOrEmpty(cols[0]))
                {
                    Log("InterfaceWindowSounds: parse error path=\"" + pathForErrors
                        + "\" line=" + lineNo + " expected window, open, close");
                    continue;
                }

                ByWindow[cols[0]] = (cols[1], cols[2]);
            }
        }
    }

    public static string OpenToken(string windowType)
    {
        EnsureLoaded();
        if (TryGet(windowType, out var row))
            return row.open;
        Log(FormatUnknownWindowError(windowType));
        return null;
    }

    public static string CloseToken(string windowType)
    {
        EnsureLoaded();
        if (TryGet(windowType, out var row))
            return row.close;
        Log(FormatUnknownWindowError(windowType));
        return null;
    }

    static bool TryGet(string windowType, out (string open, string close) row)
    {
        if (!string.IsNullOrEmpty(windowType) && ByWindow.TryGetValue(windowType, out row))
            return true;
        return ByWindow.TryGetValue(DefaultWindow, out row);
    }

    static void EnsureLoaded()
    {
        if (!_loaded)
            Load();
    }

    static string DefaultPath() =>
        Path.Combine(Application.streamingAssetsPath, RelativePath);

    static string ResolveDefaultText()
    {
        if (TextOverride != null)
            return TextOverride();
        string path = DefaultPath();
        return File.Exists(path) ? File.ReadAllText(path) : null;
    }

    static void Log(string message)
    {
        ErrorLogged?.Invoke(message);
        Debug.LogError(message);
    }
}
