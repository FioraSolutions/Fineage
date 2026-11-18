using System;

/// <summary>Which Fineage-Backend a run dials.</summary>
public enum FineageBackend
{
    /// <summary>The shared remote playtest (login/game on evpoint.by). The default.</summary>
    Remote,

    /// <summary>Fineage-Backend on this machine (login 9106 / zone 9777). Opt-in.</summary>
    Local,
}

/// <summary>
/// Explicit environment choice, in one place both the Editor and a player build read.
///
/// <para>
/// <b>Remote is the default.</b> Local is opt-in and never implicit:
/// <list type="number">
/// <item>Command line: <c>-fineageBackend local</c> / <c>--fineage-backend=remote</c>.</item>
/// <item><c>FINEAGE_BACKEND</c> — <c>local</c> / <c>remote</c>.</item>
/// <item><c>FINEAGE_NET_CLIENT</c> — the legacy local launch-script switch.</item>
/// <item>A stored choice (EditorPrefs, Editor only; Fineage/Play Local remembers it).</item>
/// <item><paramref name="fallback"/> — Local in the Editor, Remote in a player build.</item>
/// </list>
/// </para>
///
/// <para>
/// A release-clients player is compiled without <c>FINEAGE_LOCAL_CAPABLE</c>
/// (<see cref="FineageServerProfile.LocalOverridesAllowed"/>), so nothing above the default is
/// honoured there: GitHub Actions always dials the remote tunnel, whatever the runner
/// environment says.
/// </para>
/// </summary>
public static class FineageBackendSelector
{
    /// <summary>Environment variable that names the backend outright.</summary>
    public const string BackendEnv = "FINEAGE_BACKEND";

    /// <summary>Legacy environment variable kept for the local launch scripts.</summary>
    public const string LegacyEnv = "FINEAGE_NET_CLIENT";

    public const string CommandLineKey = "fineageBackend";
    public const string CommandLineKeyDashed = "fineage-backend";

    /// <summary>
    /// Picks the backend. When <paramref name="localOverrideAllowed"/> is false (a shipped
    /// release player) the answer is always <see cref="FineageBackend.Remote"/>.
    /// </summary>
    public static FineageBackend Resolve(
        string[] commandLine,
        string backendEnv,
        string legacyNetClientEnv,
        bool? storedChoice,
        FineageBackend fallback,
        bool localOverrideAllowed)
    {
        if (!localOverrideAllowed)
            return FineageBackend.Remote;

        FineageBackend choice;
        if (TryParseCommandLine(commandLine, out choice))
            return choice;

        if (TryParse(backendEnv, out choice))
            return choice;

        if (TryParseLegacy(legacyNetClientEnv, out choice))
            return choice;

        if (storedChoice.HasValue)
            return storedChoice.Value ? FineageBackend.Local : FineageBackend.Remote;

        return fallback;
    }

    /// <summary>Parses <c>FINEAGE_BACKEND</c>; false when unset or unrecognised.</summary>
    public static bool TryParse(string value, out FineageBackend backend)
    {
        backend = FineageBackend.Remote;
        string text = Normalize(value);
        if (text == null)
            return false;

        switch (text)
        {
            case "local":
            case "loopback":
                backend = FineageBackend.Local;
                return true;
            case "remote":
            case "public":
            case "playtest":
                backend = FineageBackend.Remote;
                return true;
            default:
                return false;
        }
    }

    /// <summary>Parses the legacy <c>FINEAGE_NET_CLIENT</c> switch; false when unset or unrecognised.</summary>
    public static bool TryParseLegacy(string value, out FineageBackend backend)
    {
        backend = FineageBackend.Remote;
        string text = Normalize(value);
        if (text == null)
            return false;

        switch (text)
        {
            case "1":
            case "true":
            case "local":
                backend = FineageBackend.Local;
                return true;
            case "0":
            case "false":
            case "remote":
            case "public":
                backend = FineageBackend.Remote;
                return true;
            default:
                return false;
        }
    }

    /// <summary>Parses <c>-fineageBackend &lt;value&gt;</c> / <c>--fineage-backend=&lt;value&gt;</c>.</summary>
    public static bool TryParseCommandLine(string[] args, out FineageBackend backend)
    {
        backend = FineageBackend.Remote;
        if (args == null)
            return false;

        for (int i = 0; i < args.Length; i++)
        {
            string arg = args[i];
            if (string.IsNullOrEmpty(arg) || arg[0] != '-')
                continue;

            string body = arg.StartsWith("--", StringComparison.Ordinal)
                ? arg.Substring(2)
                : arg.Substring(1);

            string value = null;
            int eq = body.IndexOf('=');
            if (eq >= 0)
            {
                value = body.Substring(eq + 1);
                body = body.Substring(0, eq);
            }

            if (!IsBackendKey(body))
                continue;

            if (value == null)
            {
                if (i + 1 >= args.Length || (args[i + 1].Length > 0 && args[i + 1][0] == '-'))
                    continue;
                value = args[++i];
            }

            return TryParse(value, out backend);
        }

        return false;
    }

    static bool IsBackendKey(string key)
    {
        return string.Equals(key, CommandLineKey, StringComparison.Ordinal)
            || string.Equals(key, CommandLineKeyDashed, StringComparison.Ordinal);
    }

    static string Normalize(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;
        return value.Trim().ToLowerInvariant();
    }
}
