using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Interlude CH3 SmallWnd vertical drag edge (StatusWnd, ShortcutWnd, BuffWnd, SkillTray).
/// Replaces later-chronicle Frame_DF_Ver strips; logs a concrete error when art is missing.
/// </summary>
public static class L2InterludeHudVerticalFrame
{
    public const string ColumnName = "InterludeHudVerticalColumn";
    public const string RailHostName = "InterludeHudVerticalRail";

    public const string TopResource = "Data/UI/Target/Interlude/Target_Interlude_SmallWnd_Back1";
    public const string CenterResource = "Data/UI/Target/Interlude/Target_Interlude_SmallWnd_Back2";
    public const string BottomResource = "Data/UI/Target/Interlude/Target_Interlude_SmallWnd_Back3_Ex";

    /// <summary>Shortcut drag rail uses the same CH3 SmallWnd center tile as the status plate.</summary>
    public const string DragRailResource = InterludeHudWindowCatalog.SmallWndBack2;
    public const string HorizontalResizeResource = "Data/UI/Assets/Chat/sizecontrol_icon";

    public static bool TryPaintVerticalColumn(VisualElement searchRoot, string context)
    {
        if (searchRoot == null)
        {
            Debug.LogError($"[InterludeHudFrame] Null search root for '{context}'.");
            return false;
        }

        GroupBox column = searchRoot.Q<GroupBox>(ColumnName);
        if (column == null)
        {
            VisualElement rail = searchRoot.Q<VisualElement>(RailHostName);
            column = rail?.Q<GroupBox>(ColumnName);
        }

        if (column == null)
        {
            Debug.LogError(
                $"[InterludeHudFrame] Missing GroupBox '{ColumnName}' on '{context}'. " +
                "UXML must instance Template/InterludeHudVerticalColumn.");
            return false;
        }

        while (column.childCount > 3)
            column.RemoveAt(column.childCount - 1);

        PaintColumn(column, context);
        return true;
    }

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
        L2InterludeHudSkinPipeline.StampSlice(
            element,
            DragRailResource,
            context,
            L2InterludeHudSkinPipeline.SmallWndSlice);
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
                "Import from Interlude ref/l2-interlude-client (L2UI_CH3.SmallWnd).");
            return;
        }

        element.style.backgroundImage = new StyleBackground(texture);
        element.style.unityBackgroundScaleMode = ScaleMode.StretchToFill;
    }
}