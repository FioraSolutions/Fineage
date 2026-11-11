using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

public sealed partial class PlaytestEndpoint
{
    public static PlaytestEndpoint Resolve(PlaytestEndpointInput input)
    {
        if (input == null)
            input = new PlaytestEndpointInput();

        string publicHost = DefaultPublicHost;
        string loginHost = DefaultLoginHost;
        string gameHost = DefaultGameHost;
        int loginPort = DefaultLoginPort;
        int gamePort = DefaultGamePort;
        bool gamePortSet = true;

        ApplyLayer(ParseLayer(input.BakedJson), ref publicHost, ref loginHost, ref gameHost, ref loginPort, ref gamePort, ref gamePortSet);
        ApplyLayer(ParseLayer(input.StreamingJson), ref publicHost, ref loginHost, ref gameHost, ref loginPort, ref gamePort, ref gamePortSet);
        ApplyLayer(FromEnvironment(input.Environment), ref publicHost, ref loginHost, ref gameHost, ref loginPort, ref gamePort, ref gamePortSet);
        ApplyLayer(ParseLayer(input.SidecarJson), ref publicHost, ref loginHost, ref gameHost, ref loginPort, ref gamePort, ref gamePortSet);
        ApplyLayer(ParseLayer(input.PersistentJson), ref publicHost, ref loginHost, ref gameHost, ref loginPort, ref gamePort, ref gamePortSet);
        ApplyLayer(ParseCommandLine(input.CommandLine), ref publicHost, ref loginHost, ref gameHost, ref loginPort, ref gamePort, ref gamePortSet);

        if (string.IsNullOrEmpty(loginHost))
            loginHost = publicHost;
        if (string.IsNullOrEmpty(gameHost))
            gameHost = publicHost;

        var result = new PlaytestEndpoint();
        result.PublicHost = publicHost ?? string.Empty;
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
}
