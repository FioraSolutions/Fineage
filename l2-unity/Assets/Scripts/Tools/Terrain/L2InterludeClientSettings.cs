#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Local Interlude client and UModel export paths (#476). Never commit client binaries.
/// </summary>
public static class L2InterludeClientSettings
{
    public const string PrefClientRoot = "Fineage.L2InterludeClientRoot";
    public const string PrefUModelExportRoot = "Fineage.L2UModelExportRoot";
    public const string PrefUModelSoundExportRoot = "Fineage.L2UModelSoundExportRoot";
    public const string PrefUModelSoundUnityExportRoot = "Fineage.L2UModelSoundUnityExportRoot";

    public const string EnvClientRoot = "L2_INTERLUDE_CLIENT_ROOT";
    public const string EnvUModelExportRoot = "L2_UMODEL_EXPORT_DIR";
    public const string EnvUModelSoundExportRoot = "L2_UMODEL_SOUND_EXPORT_DIR";
    public const string EnvUModelSoundUnityExportRoot = "L2_UMODEL_SOUND_UNITY_EXPORT_DIR";

    public static string ClientRoot
    {
        get => FirstNonEmpty(Environment.GetEnvironmentVariable(EnvClientRoot), EditorPrefs.GetString(PrefClientRoot, string.Empty));
        set => EditorPrefs.SetString(PrefClientRoot, value ?? string.Empty);
    }

    public static string UModelExportRoot
    {
        get => FirstNonEmpty(
            Environment.GetEnvironmentVariable(EnvUModelExportRoot),
            Environment.GetEnvironmentVariable("L2_UMODEL_MESH_DIR"),
            EditorPrefs.GetString(PrefUModelExportRoot, string.Empty));
        set => EditorPrefs.SetString(PrefUModelExportRoot, value ?? string.Empty);
    }

    public static string UModelSoundExportRoot
    {
        get => FirstNonEmpty(Environment.GetEnvironmentVariable(EnvUModelSoundExportRoot), EditorPrefs.GetString(PrefUModelSoundExportRoot, string.Empty));
        set => EditorPrefs.SetString(PrefUModelSoundExportRoot, value ?? string.Empty);
    }

    public static string UModelSoundUnityExportRoot
    {
        get => FirstNonEmpty(Environment.GetEnvironmentVariable(EnvUModelSoundUnityExportRoot), EditorPrefs.GetString(PrefUModelSoundUnityExportRoot, string.Empty));
        set => EditorPrefs.SetString(PrefUModelSoundUnityExportRoot, value ?? string.Empty);
    }

    public static string MapsDirectory =>
        string.IsNullOrEmpty(ClientRoot) ? string.Empty : Path.Combine(ClientRoot, "maps");

    public static string TexturesDirectory =>
        string.IsNullOrEmpty(ClientRoot) ? string.Empty : Path.Combine(ClientRoot, "textures");

    public static string StaticMeshesDirectory =>
        string.IsNullOrEmpty(ClientRoot) ? string.Empty : Path.Combine(ClientRoot, "staticmeshes");

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
