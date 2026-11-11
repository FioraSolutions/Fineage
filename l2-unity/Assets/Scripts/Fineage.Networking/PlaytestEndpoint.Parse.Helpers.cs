using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

public sealed partial class PlaytestEndpoint
{
    public static bool TryParseJson(string text, out PlaytestEndpointFields fields)
    {
        fields = default(PlaytestEndpointFields);
        if (string.IsNullOrWhiteSpace(text))
            return false;

        text = text.Trim();
        if (text[0] == '\uFEFF')
            text = text.Substring(1).Trim();

        int i = 0;
        Skip(text, ref i);
        if (i >= text.Length || text[i] != '{')
            return false;
        i++;

        while (true)
        {
            Skip(text, ref i);
            if (i < text.Length && text[i] == '}')
                return true;

            string key;
            if (!ReadString(text, ref i, out key))
                return false;
            Skip(text, ref i);
            if (i >= text.Length || text[i] != ':')
                return false;
            i++;
            Skip(text, ref i);
            if (!ReadValue(text, ref i, key, ref fields))
                return false;
            Skip(text, ref i);
            if (i < text.Length && text[i] == ',')
            {
                i++;
                continue;
            }
            if (i < text.Length && text[i] == '}')
                return true;
            return false;
        }
    }

    static PlaytestEndpointFields ParseLayer(string json)
    {
        PlaytestEndpointFields fields;
        if (!TryParseJson(json, out fields))
            return default(PlaytestEndpointFields);
        return fields;
    }

    static void ApplyLayer(
        PlaytestEndpointFields layer,
        ref string publicHost,
        ref string loginHost,
        ref string gameHost,
        ref int loginPort,
        ref int gamePort,
        ref bool gamePortSet)
    {
        string shared = Clean(layer.HostSet ? layer.Host : null);
        string login = Clean(layer.LoginHostSet ? layer.LoginHost : null);
        string game = Clean(layer.GameHostSet ? layer.GameHost : null);
        if (shared != null)
            publicHost = shared;
        if (login != null)
            loginHost = login;
        if (game != null)
            gameHost = game;
        if (layer.LoginPortSet && IsPort(layer.LoginPort))
            loginPort = layer.LoginPort;
        if (layer.GamePortSet && IsPort(layer.GamePort))
        {
            gamePort = layer.GamePort;
            gamePortSet = true;
        }
    }
}
