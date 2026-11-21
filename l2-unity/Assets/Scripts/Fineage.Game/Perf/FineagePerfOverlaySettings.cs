using System;

/// <summary>Resolves whether the CPU/GPU frame-timing overlay is enabled (step 1).</summary>
public static class FineagePerfOverlaySettings
{
    public const string EnvVarName = "FINEAGE_PERF_OVERLAY";
    public const string CommandLineFlag = "-perfOverlay";

    public static bool Enabled { get; private set; }

    [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Reset() => Enabled = ResolveEnabled();

    static bool ResolveEnabled()
    {
        string env = Environment.GetEnvironmentVariable(EnvVarName);
        if (!string.IsNullOrEmpty(env) && env != "0" && !string.Equals(env, "false", StringComparison.OrdinalIgnoreCase))
            return true;

        string[] args = Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (string.Equals(args[i], CommandLineFlag, StringComparison.OrdinalIgnoreCase))
                return true;
        }

#if DEVELOPMENT_BUILD || UNITY_EDITOR
        return false;
#else
        return false;
#endif
    }
}