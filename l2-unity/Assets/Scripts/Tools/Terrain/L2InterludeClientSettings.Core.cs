#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static partial class L2InterludeClientSettings
{
    public static bool TryValidateClientRoot(out string message)
    {
        if (string.IsNullOrWhiteSpace(ClientRoot))
        {
            message = "Interlude client root is empty. Set it in Shnok → Interlude batch bake.";
            return false;
        }

        if (!Directory.Exists(ClientRoot))
        {
            message = "Client root does not exist: " + ClientRoot;
            return false;
        }

        string maps = MapsDirectory;
        if (!Directory.Exists(maps))
        {
            message = "Expected maps/ under client root: " + maps;
            return false;
        }

        message = string.Empty;
        return true;
    }
}
#endif
