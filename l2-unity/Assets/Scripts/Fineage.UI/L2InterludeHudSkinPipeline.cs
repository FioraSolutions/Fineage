using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// One pipeline for in-game HUD chrome: validate Interlude texture paths, apply xdat rects,
/// and paint SmallWnd / chat slice backgrounds. Replaces per-window HF <c>Windows_DF_*</c> fills.
/// </summary>
public static class L2InterludeHudSkinPipeline
{
    public const int SmallWndSlice = 4;

    public static bool ValidateTextures(InterludeHudWindowEntry entry, string logPrefix)
    {
        bool ok = true;
        string[] paths = entry.TextureResourcePaths;
        for (int i = 0; i < paths.Length; i++)
        {
            string path = paths[i];
            if (InterludeHudWindowCatalog.IsHighFiveChromeResourcePath(path))
            {
                Debug.LogError(
                    $"[{logPrefix}] HF chrome path banned on HUD window='{entry.XdatName}' path='{path}'.");
                ok = false;
                continue;
            }

            if (Resources.Load<Texture2D>(path) == null)
            {
                Debug.LogError(
                    $"[{logPrefix}] missing Interlude HUD texture window='{entry.XdatName}' path='{path}'.");
                ok = false;
            }
        }

        return ok;
    }

    public static void ValidateRadarZoomTextures(string logPrefix)
    {
        string[] paths = InterludeHudWindowCatalog.RadarZoomTexturePaths;
        for (int i = 0; i < paths.Length; i++)
        {
            string path = paths[i];
            if (Resources.Load<Texture2D>(path) == null)
            {
                Debug.LogError(
                    $"[{logPrefix}] missing Interlude HUD texture window='RadarMapCtrl' path='{path}'.");
            }
        }
    }

    public static void PaintChatInputStrip(VisualElement innerBar, string context)
    {
        if (innerBar == null)
            return;
        StampSlice(
            innerBar,
            "Data/UI/Assets/Chat/chatting_back1",
            context,
            2);
        innerBar.style.backgroundColor = Color.clear;
    }

    public static void ApplyBuffTrayLayout(VisualElement trayRoot)
    {
        if (trayRoot == null)
            return;
        VisualElement anchor = trayRoot.Q<VisualElement>("anchor");
        if (anchor != null)
            PaintSmallWndCenterFill(anchor, nameof(InterludeHudWindowCatalog.BuffWnd));
    }

    public static void ApplyChatLayout(VisualElement chatWindowRoot)
    {
        if (chatWindowRoot == null)
            return;

        chatWindowRoot.style.backgroundColor = Color.clear;
        InterludeWindowLayout.Apply(chatWindowRoot, InterludeHudWindowCatalog.ChatWnd.XdatName);
    }

    public static void ApplyMinimapLayout(VisualElement minimapRoot)
    {
        if (minimapRoot == null)
            return;

        InterludeWindowLayout.Apply(minimapRoot, InterludeHudWindowCatalog.MinimapWnd.XdatName);
    }

    public static void PaintSmallWndCenterFill(VisualElement panel, string context)
    {
        if (panel == null)
            return;

        StampSlice(
            panel,
            InterludeHudWindowCatalog.SmallWndBack2,
            context,
            SmallWndSlice);
        panel.style.backgroundColor = Color.clear;
    }

    public static void PaintSkillbarPanel(VisualElement innerContainer, bool vertical, string context)
    {
        if (innerContainer == null)
            return;

        StampSlice(
            innerContainer,
            InterludeHudWindowCatalog.SmallWndBack2,
            context,
            SmallWndSlice);
        innerContainer.style.backgroundColor = new Color(0f, 0f, 0f, 0.15f);
        if (vertical)
        {
            innerContainer.style.paddingTop = 3;
            innerContainer.style.paddingBottom = 3;
            innerContainer.style.paddingLeft = 3;
            innerContainer.style.paddingRight = 3;
        }
        else
        {
            innerContainer.style.paddingTop = 3;
            innerContainer.style.paddingBottom = 3;
        }
    }

    public static void StampSlice(
        VisualElement element,
        string resourcePath,
        string context,
        int slice)
    {
        if (element == null)
            return;

        Texture2D texture = Resources.Load<Texture2D>(resourcePath);
        if (texture == null)
        {
            Debug.LogError(
                $"[InterludeHudSkin] missing texture path='{resourcePath}' context='{context}'.");
            return;
        }

        element.style.backgroundImage = new StyleBackground(texture);
        element.style.unityBackgroundScaleMode = ScaleMode.StretchToFill;
        element.style.unitySliceLeft = slice;
        element.style.unitySliceRight = slice;
        element.style.unitySliceTop = slice;
        element.style.unitySliceBottom = slice;
    }
}