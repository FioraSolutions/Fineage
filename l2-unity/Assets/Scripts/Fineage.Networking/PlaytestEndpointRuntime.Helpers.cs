using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static partial class PlaytestEndpointRuntime
{
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
