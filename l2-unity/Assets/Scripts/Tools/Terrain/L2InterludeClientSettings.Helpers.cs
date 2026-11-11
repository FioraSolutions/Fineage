#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static partial class L2InterludeClientSettings
{
    public static bool TryValidateUModelExportRoot(out string message)
    {
        if (string.IsNullOrWhiteSpace(UModelExportRoot))
        {
            message = "UModel export root is empty (FBX/PNG props tree).";
            return false;
        }

        if (!Directory.Exists(UModelExportRoot))
        {
            message = "UModel export root does not exist: " + UModelExportRoot;
            return false;
        }

        message = string.Empty;
        return true;
    }

    static string FirstNonEmpty(params string[] values)
    {
        for (int i = 0; i < values.Length; i++)
        {
            if (!string.IsNullOrWhiteSpace(values[i]))
            {
                return values[i].Trim();
            }
        }

        return string.Empty;
    }
}
#endif
