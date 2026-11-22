using System;

/// <summary>Cross-assembly tier queries without referencing the gameplay VisibilityManager type.</summary>
public static class VisibilityTierRegistry
{
    static Func<int, CrowdDetailTier> _getTier;
    static Func<int, bool> _shouldShowNameplate;
    static Func<string> _histogramLine;

    public static void Bind(
        Func<int, CrowdDetailTier> getTier,
        Func<int, bool> shouldShowNameplate,
        Func<string> histogramLine = null)
    {
        _getTier = getTier;
        _shouldShowNameplate = shouldShowNameplate;
        _histogramLine = histogramLine;
    }

    public static void Clear()
    {
        _getTier = null;
        _shouldShowNameplate = null;
        _histogramLine = null;
    }

    public static CrowdDetailTier GetTier(int entityId) =>
        _getTier != null ? _getTier(entityId) : CrowdDetailTier.T1Near;

    public static bool ShouldShowNameplate(int entityId) =>
        _shouldShowNameplate == null || _shouldShowNameplate(entityId);

    public static string HistogramLine => _histogramLine != null ? _histogramLine() : string.Empty;
}