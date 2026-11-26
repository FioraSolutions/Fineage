using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>Interlude musicinfo.dat tracks, keyed by nMusicID.</summary>
public static class OfficialMusicInfo
{
    public const string RelativePath = "Data/Meta/Sounds/musicinfo-tracks.tsv";

    public static Func<string> TextOverride;
    public static Action<string> ErrorLogged;

    static readonly Dictionary<int, List<string>> ById = new Dictionary<int, List<string>>();
    static bool _loaded;

    public static IReadOnlyList<string> TracksFor(int musicId)
    {
        EnsureLoaded();
        if (ById.TryGetValue(musicId, out List<string> tracks))
            return tracks;
        return Array.Empty<string>();
    }

    public static void ClearForTests()
    {
        ById.Clear();
        _loaded = false;
        TextOverride = null;
        ErrorLogged = null;
    }

    public static void LoadFrom(string text, string pathForErrors)
    {
        ById.Clear();
        _loaded = true;
        if (string.IsNullOrEmpty(text))
        {
            Log("OfficialMusicInfo: missing musicinfo table path=\"" + pathForErrors + "\"");
            return;
        }

        using (var reader = new StringReader(text))
        {
            string header = reader.ReadLine();
            if (header == null)
            {
                Log("OfficialMusicInfo: missing musicinfo table path=\"" + pathForErrors + "\"");
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
                if (cols.Length < 3 || !int.TryParse(cols[0], out int id))
                {
                    Log("OfficialMusicInfo: parse error path=\"" + pathForErrors + "\" line=" + lineNo);
                    continue;
                }

                string track = cols[2];
                if (string.IsNullOrEmpty(track))
                    continue;
                if (!ById.TryGetValue(id, out List<string> list))
                {
                    list = new List<string>();
                    ById[id] = list;
                }
                list.Add(track);
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
