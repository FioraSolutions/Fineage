using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static partial class PlaytestEndpointRuntime
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





}
