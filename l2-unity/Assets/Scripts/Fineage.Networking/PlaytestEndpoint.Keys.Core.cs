using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

public sealed partial class PlaytestEndpoint
{
    static void AssignKey(ref PlaytestEndpointFields fields, string key, string value)
    {
        if (IsKey(key, "fineageHost", "fineage-host"))
        {
            fields.Host = value;
            fields.HostSet = true;
        }
        else if (IsKey(key, "fineageLoginHost", "fineage-login-host"))
        {
            fields.LoginHost = value;
            fields.LoginHostSet = true;
        }
        else if (IsKey(key, "fineageGameHost", "fineage-game-host"))
        {
            fields.GameHost = value;
            fields.GameHostSet = true;
        }
        else if (IsKey(key, "fineageLoginPort", "fineage-login-port"))
        {
            AssignPort(value, out fields.LoginPort, out fields.LoginPortSet);
        }
        else if (IsKey(key, "fineageGamePort", "fineage-game-port"))
        {
            AssignPort(value, out fields.GamePort, out fields.GamePortSet);
        }
    }

    static bool TrySplitArg(string arg, out string key, out string value)
    {
        key = null;
        value = null;
        if (string.IsNullOrEmpty(arg))
            return false;

        string body = arg;
        if (body.StartsWith("--", StringComparison.Ordinal))
            body = body.Substring(2);
        else if (body.StartsWith("-", StringComparison.Ordinal))
            body = body.Substring(1);
        else
            return false;

        int eq = body.IndexOf('=');
        if (eq >= 0)
        {
            key = body.Substring(0, eq);
            value = body.Substring(eq + 1);
        }
        else
        {
            key = body;
        }
        return IsKnownKey(key);
    }

    static bool IsKnownKey(string key)
    {
        return IsKey(key, "fineageHost", "fineage-host")
            || IsKey(key, "fineageLoginHost", "fineage-login-host")
            || IsKey(key, "fineageGameHost", "fineage-game-host")
            || IsKey(key, "fineageLoginPort", "fineage-login-port")
            || IsKey(key, "fineageGamePort", "fineage-game-port");
    }

    static bool IsKey(string key, string camel, string dashed)
    {
        return string.Equals(key, camel, StringComparison.Ordinal)
            || string.Equals(key, dashed, StringComparison.Ordinal);
    }

    static bool IsDashArg(string arg)
    {
        return !string.IsNullOrEmpty(arg) && arg[0] == '-';
    }
}
