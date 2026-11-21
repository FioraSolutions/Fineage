using System;
using UnityEngine.UIElements;

/// <summary>Interlude MagicSkillWnd grid metrics (32px icon, standard slot frame, row gap).</summary>
public static class L2SkillsPanelLayout
{
    public const float SkillIconSize = 32f;
    public const float SlotOuterSize = 34f;
    public const float SlotGap = 2f;
    public const float RowHorizontalInset = 1f;
    public const float RowHeight = 36f;
    public const int MinColumns = 1;
    public const int MaxColumns = 7;
    public const int DefaultColumns = 6;

    public const string RowFrame6 = "Data/UI/Window/Skills/Skills_SkillBar_6x1";
    public const string RowFrame7 = "Data/UI/Window/Skills/Skills_SkillBar_2x2";

    public static int ComputeColumns(float panelContentWidth)
    {
        if (panelContentWidth <= 0f)
            return DefaultColumns;

        float usable = panelContentWidth - RowHorizontalInset * 2f;
        int columns = (int)Math.Floor((usable + SlotGap) / (SlotOuterSize + SlotGap));
        return Math.Clamp(columns, MinColumns, MaxColumns);
    }

    public static float ComputePanelWidth(int columns)
    {
        if (columns < MinColumns)
            columns = MinColumns;
        return RowHorizontalInset * 2f + columns * SlotOuterSize + Math.Max(0, columns - 1) * SlotGap;
    }

    public static string RowFrameResourceForColumns(int columns) =>
        columns <= 6 ? RowFrame6 : RowFrame7;

    /// <summary>
    /// InventoryTab.uxml bakes height:1px for the item grid; MagicSkillWnd clears its scroll child but keeps that host.
    /// </summary>
    public static void ApplyMagicSkillTabHostLayout(VisualElement tabHost)
    {
        if (tabHost == null)
            return;

        tabHost.style.height = StyleKeyword.Auto;
        tabHost.style.minHeight = StyleKeyword.Auto;
        tabHost.style.maxHeight = StyleKeyword.Null;
        tabHost.style.flexGrow = 1f;
        tabHost.style.flexShrink = 0f;
        tabHost.style.flexDirection = FlexDirection.Column;

        VisualElement content = tabHost.Q("Content");
        if (content == null)
            return;

        content.style.height = StyleKeyword.Auto;
        content.style.minHeight = StyleKeyword.Auto;
        content.style.flexGrow = 1f;
        content.style.flexShrink = 0f;
        content.style.flexDirection = FlexDirection.Column;
    }

    public static void ApplyMagicSkillTabHostsFromPanel(VisualElement skillsPanelRoot)
    {
        if (skillsPanelRoot == null)
            return;

        VisualElement tabContainer = skillsPanelRoot.Q("tab-content-container");
        if (tabContainer == null)
            return;

        for (int i = 0; i < tabContainer.childCount; i++)
            ApplyMagicSkillTabHostLayout(tabContainer[i]);
    }

    public static void EnsureInterludeGridRowMetrics(VisualElement root)
    {
        if (root == null)
            return;

        WalkGridRows(root);
    }

    static void WalkGridRows(VisualElement node)
    {
        if (node.ClassListContains("skills-grid-row"))
            ApplyGridRowMetrics(node);

        for (int i = 0; i < node.childCount; i++)
            WalkGridRows(node[i]);
    }

    public static void ApplyGridRowMetrics(VisualElement rowRoot)
    {
        if (rowRoot == null)
            return;

        if (!rowRoot.ClassListContains("skills-grid-row"))
            rowRoot.AddToClassList("skills-grid-row");

        rowRoot.style.height = RowHeight;
        rowRoot.style.minHeight = RowHeight;
        rowRoot.style.maxHeight = RowHeight;
        rowRoot.style.flexShrink = 0f;
        rowRoot.style.flexGrow = 0f;
    }

  /// <summary>Inline/grid style height; layout may be 0 in EditMode until a panel exists.</summary>
    public static float ReadConfiguredRowHeight(VisualElement rowRoot)
    {
        if (rowRoot == null)
            return 0f;

        float minH = rowRoot.style.minHeight.value.value;
        if (minH > 0f)
            return minH;

        float h = rowRoot.style.height.value.value;
        return h > 0f ? h : 0f;
    }
}