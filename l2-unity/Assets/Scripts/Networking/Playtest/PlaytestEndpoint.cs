#nullable disable
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

/// <summary>
/// Resolved shared-playtest login and game endpoint.
/// Empty hosts mean "keep the serialized / server-list address".
/// </summary>
public sealed class PlaytestEndpoint
{
    public const int DefaultLoginPort = 2106;

    public string LoginHost { get; private set; }
    public string GameHost { get; private set; }
    public int LoginPort { get; private set; }
    public int GamePort { get; private set; }
    public bool OverrideLogin { get; private set; }
    public bool OverrideGameHost { get; private set; }
    public bool OverrideGamePort { get; private set; }

    /// <summary>
    /// Layers are applied lowest priority first. An empty host does not clear a lower layer.
    /// Login port defaults to 2106 when a login host is set and no port is.
    /// Game port stays on the server-list value unless a port in 1..65535 is set.
    /// </summary>
    public static PlaytestEndpoint Resolve(PlaytestEndpointInput input)
    {
        if (input == null)
            input = new PlaytestEndpointInput();

        string loginHost = null;
        string gameHost = null;
        int loginPort = 0;
        int gamePort = 0;
        bool gamePortSet = false;

        ApplyLayer(ParseLayer(input.BakedJson), ref loginHost, ref gameHost, ref loginPort, ref gamePort, ref gamePortSet);
        ApplyLayer(ParseLayer(input.StreamingJson), ref loginHost, ref gameHost, ref loginPort, ref gamePort, ref gamePortSet);
        ApplyLayer(FromEnvironment(input.Environment), ref loginHost, ref gameHost, ref loginPort, ref gamePort, ref gamePortSet);
        ApplyLayer(ParseLayer(input.SidecarJson), ref loginHost, ref gameHost, ref loginPort, ref gamePort, ref gamePortSet);
        ApplyLayer(ParseLayer(input.PersistentJson), ref loginHost, ref gameHost, ref loginPort, ref gamePort, ref gamePortSet);
        ApplyLayer(ParseCommandLine(input.CommandLine), ref loginHost, ref gameHost, ref loginPort, ref gamePort, ref gamePortSet);

        var result = new PlaytestEndpoint();
        result.LoginHost = loginHost ?? string.Empty;
        result.GameHost = gameHost ?? string.Empty;
        result.OverrideLogin = !string.IsNullOrEmpty(loginHost);
        result.OverrideGameHost = !string.IsNullOrEmpty(gameHost);
        result.LoginPort = result.OverrideLogin && loginPort <= 0 ? DefaultLoginPort : loginPort;
        result.OverrideGamePort = gamePortSet && gamePort > 0;
        result.GamePort = result.OverrideGamePort ? gamePort : 0;
        if (!result.OverrideLogin)
            result.LoginPort = 0;
        return result;
    }

    public static PlaytestEndpointFields FromEnvironment(IDictionary<string, string> env)
    {
        PlaytestEndpointFields fields = default(PlaytestEndpointFields);
        if (env == null)
            return fields;

        string value;
        if (TryGet(env, "FINEAGE_PUBLIC_HOST", out value))
        {
            fields.Host = value;
            fields.HostSet = true;
        }
        if (TryGet(env, "FINEAGE_LOGIN_HOST", out value))
        {
            fields.LoginHost = value;
            fields.LoginHostSet = true;
        }
        if (TryGet(env, "FINEAGE_GAME_HOST", out value))
        {
            fields.GameHost = value;
            fields.GameHostSet = true;
        }
        if (TryGet(env, "FINEAGE_LOGIN_PORT", out value))
            AssignPort(value, out fields.LoginPort, out fields.LoginPortSet);
        if (TryGet(env, "FINEAGE_GAME_PORT", out value))
            AssignPort(value, out fields.GamePort, out fields.GamePortSet);
        return fields;
    }

    public static PlaytestEndpointFields ParseCommandLine(string[] args)
    {
        PlaytestEndpointFields fields = default(PlaytestEndpointFields);
        if (args == null)
            return fields;

        for (int i = 0; i < args.Length; i++)
        {
            string key;
            string value;
            if (!TrySplitArg(args[i], out key, out value))
                continue;
            if (value == null)
            {
                if (i + 1 >= args.Length || IsDashArg(args[i + 1]))
                    continue;
                value = args[++i];
            }
            AssignKey(ref fields, key, value);
        }
        return fields;
    }

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
        ref string loginHost,
        ref string gameHost,
        ref int loginPort,
        ref int gamePort,
        ref bool gamePortSet)
    {
        string shared = Clean(layer.HostSet ? layer.Host : null);
        string login = Clean(layer.LoginHostSet ? layer.LoginHost : null);
        string game = Clean(layer.GameHostSet ? layer.GameHost : null);
        if (login == null)
            login = shared;
        if (game == null)
            game = shared;
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

    static bool ReadNumber(string text, ref int i, out string token)
    {
        int start = i;
        if (text[i] == '-')
            i++;
        int digits = i;
        while (i < text.Length && (char.IsDigit(text[i]) || text[i] == '.'))
            i++;
        token = text.Substring(start, i - start);
        return i > digits && token != "-";
    }

    static bool MatchLiteral(string text, ref int i, string literal)
    {
        if (i + literal.Length > text.Length)
            return false;
        if (string.Compare(text, i, literal, 0, literal.Length, StringComparison.Ordinal) != 0)
            return false;
        i += literal.Length;
        return true;
    }

    static bool ReadString(string text, ref int i, out string value)
    {
        value = null;
        if (i >= text.Length || text[i] != '"')
            return false;
        i++;
        var buffer = new System.Text.StringBuilder();
        while (i < text.Length)
        {
            char c = text[i++];
            if (c == '"')
            {
                value = buffer.ToString();
                return true;
            }
            if (c != '\\')
            {
                buffer.Append(c);
                continue;
            }
            if (i >= text.Length)
                return false;
            char esc = text[i++];
            switch (esc)
            {
                case '"':
                case '\\':
                case '/':
                    buffer.Append(esc);
                    break;
                case 'b':
                    buffer.Append('\b');
                    break;
                case 'f':
                    buffer.Append('\f');
                    break;
                case 'n':
                    buffer.Append('\n');
                    break;
                case 'r':
                    buffer.Append('\r');
                    break;
                case 't':
                    buffer.Append('\t');
                    break;
                case 'u':
                    if (i + 4 > text.Length)
                        return false;
                    int code;
                    if (!int.TryParse(text.Substring(i, 4), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out code))
                        return false;
                    buffer.Append((char)code);
                    i += 4;
                    break;
                default:
                    return false;
            }
        }
        return false;
    }
}

public struct PlaytestEndpointFields
{
    public string Host;
    public string LoginHost;
    public string GameHost;
    public int LoginPort;
    public int GamePort;
    public bool HostSet;
    public bool LoginHostSet;
    public bool GameHostSet;
    public bool LoginPortSet;
    public bool GamePortSet;
}

public sealed class PlaytestEndpointInput
{
    public string[] CommandLine;
    public IDictionary<string, string> Environment;
    public string PersistentJson;
    public string SidecarJson;
    public string StreamingJson;
    public string BakedJson;
}

/// <summary>
/// Literal IPs pass through. Hostnames are resolved, IPv4 first, so a tunnel DNS name can be dialed.
/// </summary>
public static class EndpointAddress
{
    public static IPAddress Resolve(string host)
    {
        if (string.IsNullOrWhiteSpace(host))
            throw new ArgumentException("Server host is empty.");

        host = host.Trim();
        IPAddress parsed;
        if (IPAddress.TryParse(host, out parsed))
            return parsed;

        IPAddress[] addresses;
        try
        {
            addresses = Dns.GetHostAddresses(host);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Could not resolve server host '" + host + "'.", ex);
        }

        IPAddress any = null;
        for (int i = 0; i < addresses.Length; i++)
        {
            if (any == null)
                any = addresses[i];
            if (addresses[i].AddressFamily == AddressFamily.InterNetwork)
                return addresses[i];
        }
        if (any != null)
            return any;
        throw new InvalidOperationException("Could not resolve server host '" + host + "'.");
    }
}
