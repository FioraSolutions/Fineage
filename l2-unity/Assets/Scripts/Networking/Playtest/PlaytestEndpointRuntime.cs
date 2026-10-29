using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Reads optional public-RP host/port overrides (baked JSON, StreamingAssets, env, sidecar,
/// user file, command line). <see cref="FineageEndpoints.Public"/> turns them into endpoints.
/// </summary>
public static class PlaytestEndpointRuntime
{
    public const string FileName = "fineage-endpoint.json";
    public const string ResourceName = "FineagePlaytest";

    static PlaytestEndpoint _current;

    public static PlaytestEndpoint Current
    {
        get
        {
            if (_current == null)
                _current = ResolveSafe();
            return _current;
        }
    }

    static PlaytestEndpoint ResolveSafe()
    {
        try
        {
            var input = new PlaytestEndpointInput();
            input.CommandLine = Environment.GetCommandLineArgs();
            input.Environment = ReadEnvironment();
            input.PersistentJson = ReadPersistent();
            input.SidecarJson = ReadSidecar();
            input.StreamingJson = ReadStreaming();
            input.BakedJson = ReadBaked();
            return PlaytestEndpoint.Resolve(input);
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Playtest endpoint was not applied: " + ex.Message);
            return PlaytestEndpoint.Resolve(new PlaytestEndpointInput());
        }
    }

    static Dictionary<string, string> ReadEnvironment()
    {
        var result = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (DictionaryEntry entry in Environment.GetEnvironmentVariables())
        {
            string key = entry.Key as string;
            string value = entry.Value as string;
            if (key != null && value != null)
                result[key] = value;
        }
        return result;
    }

    static string ReadPersistent()
    {
        try
        {
            return ReadFile(Path.Combine(Application.persistentDataPath, FileName));
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Playtest user endpoint was not read: " + ex.Message);
            return null;
        }
    }

    static string ReadSidecar()
    {
        try
        {
            string dataPath = Application.dataPath;
            DirectoryInfo parent = Directory.GetParent(dataPath);
            if (parent != null)
            {
                string direct = ReadFile(Path.Combine(parent.FullName, FileName));
                if (direct != null)
                    return direct;
            }

            DirectoryInfo dir = parent;
            while (dir != null)
            {
                if (dir.Name.EndsWith(".app", StringComparison.Ordinal) && dir.Parent != null)
                    return ReadFile(Path.Combine(dir.Parent.FullName, FileName));
                dir = dir.Parent;
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Playtest sidecar was not read: " + ex.Message);
        }
        return null;
    }

    static string ReadStreaming()
    {
        try
        {
            string path = Path.Combine(Application.streamingAssetsPath, FileName);
            if (path.IndexOf("://", StringComparison.Ordinal) >= 0)
                return null;
            return ReadFile(path);
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Playtest StreamingAssets endpoint was not read: " + ex.Message);
            return null;
        }
    }

    static string ReadBaked()
    {
        try
        {
            TextAsset asset = Resources.Load<TextAsset>(ResourceName);
            return asset != null ? asset.text : null;
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Playtest baked endpoint was not read: " + ex.Message);
            return null;
        }
    }

    static string ReadFile(string path)
    {
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
            return null;
        return File.ReadAllText(path);
    }
}
