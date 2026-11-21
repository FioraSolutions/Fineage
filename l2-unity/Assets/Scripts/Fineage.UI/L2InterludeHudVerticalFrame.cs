using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Interlude CH3 SmallWnd vertical drag edge (StatusWnd, ShortcutWnd, BuffWnd, SkillTray).
/// Replaces later-chronicle Frame_DF_Ver strips; logs a concrete error when art is missing.
/// </summary>
public static class L2InterludeHudVerticalFrame
{
    public const string TopResource = "Data/UI/Target/Interlude/Target_Interlude_SmallWnd_Back1";
    public const string CenterResource = "Data/UI/Target/Interlude/Target_Interlude_SmallWnd_Back2";
    public const string BottomResource = "Data/UI/Target/Interlude/Target_Interlude_SmallWnd_Back3_Ex";
    public const string ColumnBgResource = "Data/UI/Assets/Windows/Windows_DF_Small_Vertical_Bg";
    public const string HorizontalResizeResource = "Data/UI/Assets/Chat/sizecontrol_icon";

    /// <summary>Paint a 3-piece vertical column (top/center/bottom).</summary>
    public static void PaintColumn(GroupBox column, string context)
    {
        if (column == null || column.childCount < 3)
        {
            Debug.LogError(
                $"[InterludeHudFrame] Missing vertical frame column on '{context}'. " +
                "Expected GroupBox with 3 children (top/center/bottom).");
            return;
        }

        StampSlice(column[0], TopResource, context);
        StampSlice(column[1], CenterResource, context);
        StampSlice(column[2], BottomResource, context);
    }

    /// <summary>Single-slice vertical bar (shortcut drag rail).</summary>
    public static void PaintDragRail(VisualElement element, string context)
    {
        StampSlice(element, ColumnBgResource, context);
        if (element != null)
        {
            element.style.unitySliceLeft = 4;
            element.style.unitySliceRight = 4;
            element.style.unitySliceTop = 4;
            element.style.unitySliceBottom = 4;
        }
    }

    public static void StampHorizontalResize(VisualElement grip, string context)
    {
        StampSlice(grip, HorizontalResizeResource, context);
    }

    static void StampSlice(VisualElement element, string resourcePath, string context)
    {
        if (element == null)
            return;

        Texture2D texture = Resources.Load<Texture2D>(resourcePath);
        if (texture == null)
        {
            Debug.LogError(
                $"[InterludeHudFrame] Missing texture path='{resourcePath}' context='{context}'. " +
                "Import from Interlude ref/l2-interlude-client (L2UI_CH3.SmallWnd / l2ui_ct1.Windows_DF_Small_Vertical_Bg).");
            return;
        }

        element.style.backgroundImage = new StyleBackground(texture);
        element.style.unityBackgroundScaleMode = ScaleMode.StretchToFill;
    }
}