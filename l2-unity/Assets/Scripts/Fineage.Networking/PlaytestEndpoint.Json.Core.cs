using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

public sealed partial class PlaytestEndpoint
{
    static bool ReadValue(string text, ref int i, string key, ref PlaytestEndpointFields fields)
    {
        if (i >= text.Length)
            return false;
        if (text[i] == '"')
        {
            string value;
            if (!ReadString(text, ref i, out value))
                return false;
            AssignJsonString(ref fields, key, value);
            return true;
        }
        if (text[i] == '-' || char.IsDigit(text[i]))
        {
            string token;
            if (!ReadNumber(text, ref i, out token))
                return false;
            AssignJsonNumber(ref fields, key, token);
            return true;
        }
        if (MatchLiteral(text, ref i, "null"))
            return true;
        return false;
    }

    static void AssignJsonString(ref PlaytestEndpointFields fields, string key, string value)
    {
        if (key == "host")
        {
            fields.Host = value;
            fields.HostSet = true;
        }
        else if (key == "loginHost")
        {
            fields.LoginHost = value;
            fields.LoginHostSet = true;
        }
        else if (key == "gameHost")
        {
            fields.GameHost = value;
            fields.GameHostSet = true;
        }
        else if (key == "loginPort")
        {
            AssignPort(value, out fields.LoginPort, out fields.LoginPortSet);
        }
        else if (key == "gamePort")
        {
            AssignPort(value, out fields.GamePort, out fields.GamePortSet);
        }
    }

    static void AssignJsonNumber(ref PlaytestEndpointFields fields, string key, string token)
    {
        if (key == "loginPort")
            AssignPort(token, out fields.LoginPort, out fields.LoginPortSet);
        else if (key == "gamePort")
            AssignPort(token, out fields.GamePort, out fields.GamePortSet);
    }
}
