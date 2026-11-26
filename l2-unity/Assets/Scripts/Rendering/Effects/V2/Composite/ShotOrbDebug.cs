using UnityEngine;

/// <summary>Opt-in diagnostics for composite shot-orbs (default off — was spamming every spawn).</summary>
public static class ShotOrbDebug
{
    public static bool Verbose;

    public static void Log(string message)
    {
        if (Verbose)
            Debug.Log(message);
    }

    public static void LogWarning(string message)
    {
        if (Verbose)
            Debug.LogWarning(message);
    }
}