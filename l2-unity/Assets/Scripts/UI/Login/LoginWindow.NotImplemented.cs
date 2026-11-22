using UnityEngine;

public partial class LoginWindow
{
    internal static void LogLoginNotImplemented(string controlName, string detail)
    {
        Debug.LogError("[LoginWindow] " + controlName + " is not implemented: " + detail);
    }
}