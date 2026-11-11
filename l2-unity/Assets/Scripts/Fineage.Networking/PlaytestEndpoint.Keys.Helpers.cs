using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

public sealed partial class PlaytestEndpoint
{
    static void AssignPort(string text, out int port, out bool set)
    {
        port = 0;
        set = false;
        if (string.IsNullOrWhiteSpace(text))
            return;
        int parsed;
        if (!int.TryParse(text.Trim(), out parsed))
            return;
        port = parsed;
        set = true;
    }

    static bool TryGet(IDictionary<string, string> env, string key, out string value)
    {
        value = null;
        if (!env.ContainsKey(key) || env[key] == null)
            return false;
        value = env[key];
        return true;
    }

    static string Clean(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;
        return value.Trim();
    }

    static bool IsPort(int port)
    {
        return port > 0 && port <= 65535;
    }

    static void Skip(string text, ref int i)
    {
        while (i < text.Length && char.IsWhiteSpace(text[i]))
            i++;
    }
}
