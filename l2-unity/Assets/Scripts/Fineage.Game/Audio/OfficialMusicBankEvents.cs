using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Confirmed Music.bank event paths, keyed by official musicinfo track name.
/// A track with no row is missing — do not invent a substitute event.
/// </summary>
public static class OfficialMusicBankEvents
{
    public const string RelativePath = "Data/Meta/Sounds/official-music-bank-events.tsv";

    public static Func<string> TextOverride;
    public static Action<string> ErrorLogged;

    static readonly Dictionary<string, string> ByTrack =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    static bool _loaded;

    public static bool TryResolve(string track, out string eventPath, out string error)
    {
        EnsureLoaded();
        eventPath = null;
        error = null;
        if (string.IsNullOrEmpty(track))
        {
            error = FormatMissingError(track);
            return false;
        }
        if (ByTrack.TryGetValue(track, out eventPath) && !string.IsNullOrEmpty(eventPath))
            return true;
        error = FormatMissingError(track);
        eventPath = null;
        return false;
    }

    public static string FormatMissingError(string track) =>
        "OfficialMusicBankEvents: no Music.bank event for track=\"" + (track ?? "") + "\"";

    public static void ClearForTests()
    {
        ByTrack.Clear();
        _loaded = false;
        TextOverride = null;
        ErrorLogged = null;
    }

    public static void LoadFrom(string text, string pathForErrors)
    {
        ByTrack.Clear();
        _loaded = true;
        if (string.IsNullOrEmpty(text))
        {
            Log("OfficialMusicBankEvents: missing table path=\"" + pathForErrors + "\"");
            return;
        }

        using (var reader = new StringReader(text))
        {
            string header = reader.ReadLine();
            if (header == null)
            {
                Log("OfficialMusicBankEvents: missing table path=\"" + pathForErrors + "\"");
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
                if (cols.Length < 2 || string.IsNullOrEmpty(cols[0]) || string.IsNullOrEmpty(cols[1]))
                {
                    Log("OfficialMusicBankEvents: parse error path=\"" + pathForErrors + "\" line=" + lineNo);
                    continue;
                }
                ByTrack[cols[0]] = cols[1];
            }
        }
    }

    static void EnsureLoaded()
    {
        if (_loaded)
            return;
        string path = Path.Combine(Application.streamingAssetsPath, RelativePath);
        string text = TextOverride != null ? TextOverride() : (File.Exists(path) ? File.ReadAllText(path) : null);
        LoadFrom(text, path);
    }

    static void Log(string message)
    {
        ErrorLogged?.Invoke(message);
        Debug.LogError(message);
    }
}
