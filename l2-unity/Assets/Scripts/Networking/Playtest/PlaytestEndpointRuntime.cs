using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Loads the shared playtest endpoint and applies it to login/game sockets.
/// Shipped default is the JSON baked by CI. Command line, user file, and env override it.
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

    /// <summary>Drop the cached endpoint so the next read reloads JSON / env / cmdline layers.</summary>
    public static PlaytestEndpoint Reload()
    {
        _current = null;
        return Current;
    }

    public static void ApplyLogin(DefaultClient client)
    {
        if (client == null)
            return;

        PlaytestEndpoint endpoint = Current;
        if (!endpoint.OverrideLogin)
            return;

        client.ServerIp = endpoint.LoginHost;
        client.ServerPort = endpoint.LoginPort;
        LobbyFlowLog.Info("Playtest login " + endpoint.LoginHost + ":" + endpoint.LoginPort);
    }

    public static void ApplyGame(DefaultClient client)
    {
        if (client == null)
            return;

        PlaytestEndpoint endpoint = Current;
        if (!endpoint.OverrideGameHost && !endpoint.OverrideGamePort)
            return;

        // ServerList often advertises a LAN address. One public host replaces it.
        if (endpoint.OverrideGameHost)
            client.ServerIp = endpoint.GameHost;
        if (endpoint.OverrideGamePort)
            client.ServerPort = endpoint.GamePort;
        LobbyFlowLog.Info("Playtest game " + client.ServerIp + ":" + client.ServerPort);
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
