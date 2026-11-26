using UnityEngine;

/// <summary>
/// Interlude NCLoginFunctionWnd actions without a ported backend log here (not via LoginWindow UI state).
/// </summary>
public static class InterludeLoginNotImplemented
{
    public const string LogTag = "[LoginWindow]";

    public static void Log(string controlName, string detail)
    {
        Debug.LogError(LogTag + " " + controlName + " is not implemented: " + detail);
    }
}