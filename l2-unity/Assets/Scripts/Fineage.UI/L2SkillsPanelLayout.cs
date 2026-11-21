using System;

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
}