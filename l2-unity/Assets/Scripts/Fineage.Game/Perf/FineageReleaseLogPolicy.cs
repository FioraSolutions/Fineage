using UnityEngine;

/// <summary>
/// Roadmap step 5: release player builds avoid stack traces on hot Log/Warning paths.
/// </summary>
public static class FineageReleaseLogPolicy
{
    public const StackTraceLogType ReleaseLogStackTrace = StackTraceLogType.None;
    public const StackTraceLogType ReleaseWarningStackTrace = StackTraceLogType.None;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ApplyReleaseStackTraces()
    {
#if UNITY_EDITOR
        return;
#else
        if (Debug.isDebugBuild)
            return;

        Application.SetStackTraceLogType(LogType.Log, ReleaseLogStackTrace);
        Application.SetStackTraceLogType(LogType.Warning, ReleaseWarningStackTrace);
#endif
    }
}