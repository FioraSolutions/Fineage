using UnityEngine;

/// <summary>Resources loads for UI chrome — logs a concrete error when art is missing (no silent fallbacks).</summary>
public static class L2UiResources
{
    public static Texture2D LoadTexture(string resourcesPath)
    {
        if (string.IsNullOrEmpty(resourcesPath))
        {
            Debug.LogError("[L2Ui] LoadTexture called with an empty Resources path.");
            return null;
        }

        Texture2D texture = Resources.Load<Texture2D>(resourcesPath);
        if (texture == null)
            Debug.LogError($"[L2Ui] Missing texture at Resources/{resourcesPath}");
        return texture;
    }
}