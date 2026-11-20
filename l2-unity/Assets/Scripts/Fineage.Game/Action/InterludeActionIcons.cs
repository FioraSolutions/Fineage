using System;
using UnityEngine;

/// <summary>
/// Resolves Interlude ActionWnd icons from catalog stems; logs concrete misses (no silent placeholder).
/// </summary>
public static class InterludeActionIcons
{
    const string ButtonImgPrefix = "Data/UI/Window/Action/Row/button_img/";
    const string SysIconPrefix = "Data/SysTextures/Icon/";

    /// <summary>Primary + fallback search paths for an action icon stem.</summary>
    public static readonly string[] SearchPrefixes =
    {
        ButtonImgPrefix,
        SysIconPrefix,
    };

    public static string ResolveIconStem(int actionId)
    {
        if (InterludeActionCatalog.TryGetIconStem(actionId, out string stem))
            return stem;
        return null;
    }

    public static bool TryResolveResourcesPath(int actionId, out string resourcesPath)
    {
        resourcesPath = null;
        string stem = ResolveIconStem(actionId);
        if (string.IsNullOrEmpty(stem))
        {
            Debug.LogError(
                "[ActionIcon] Missing catalog icon for action id=" + actionId +
                "; expected ActionName/ActionWnd entry.");
            return false;
        }

        for (int i = 0; i < SearchPrefixes.Length; i++)
        {
            string candidate = SearchPrefixes[i] + stem;
            if (Resources.Load<Texture2D>(candidate) != null)
            {
                resourcesPath = candidate;
                return true;
            }
        }

        Debug.LogError(
            "[ActionIcon] action id=" + actionId + " icon stem '" + stem +
            "' not found under " + ButtonImgPrefix + " or " + SysIconPrefix + ".");
        return false;
    }

    /// <summary>EditMode: texture exists for catalog stem (no log).</summary>
    public static bool StemResolvesInResources(string iconStem)
    {
        if (string.IsNullOrEmpty(iconStem))
            return false;

        for (int i = 0; i < SearchPrefixes.Length; i++)
        {
            if (Resources.Load<Texture2D>(SearchPrefixes[i] + iconStem) != null)
                return true;
        }

        return false;
    }
}