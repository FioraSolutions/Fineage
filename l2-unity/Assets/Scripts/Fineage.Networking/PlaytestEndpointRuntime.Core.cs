using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static partial class PlaytestEndpointRuntime
{
    public static PlaytestEndpoint Reload()
    {
        _current = null;
        return Current;
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
}
