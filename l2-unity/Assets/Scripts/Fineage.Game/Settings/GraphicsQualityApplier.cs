using UnityEngine;
using UnityEngine.Rendering;

/// <summary>Applies graphics quality through QualitySettings and the catalog profile (single pipeline for boot and user changes).</summary>
public static class GraphicsQualityApplier
{
    const string LogPrefix = "GraphicsQuality";

    static GraphicsQualityCatalog _catalog;

    public static GraphicsQualityCatalog Catalog
    {
        get
        {
            if (_catalog == null)
                _catalog = LoadCatalog();
            return _catalog;
        }
    }

    public static int ResolveBootQualityIndex()
    {
        if (GameSettings.HasSavedGraphicsQuality)
            return GameSettings.GraphicsQualityLevelIndex;

        GraphicsPlatformGroup group = GraphicsQualityPlatformTable.ResolveOrDefault(Application.platform);
        int index = Catalog.ResolveDefaultQualityIndex(group);
        FineageLog.Info(
            FineageLogCategory.World,
            LogPrefix + ": boot default tier index " + index + " (" + QualitySettings.names[index] +
            ") for platform group " + group + " on " + Application.platform);
        return index;
    }

    public static void ApplyQualityLevel(int qualityLevelIndex, bool logOnSuccess = true)
    {
        string[] names = QualitySettings.names;
        if (qualityLevelIndex < 0 || qualityLevelIndex >= names.Length)
        {
            FineageLog.Error(
                FineageLogCategory.World,
                LogPrefix + ": invalid quality level index " + qualityLevelIndex +
                " (project has " + names.Length + " tiers)");
            return;
        }

        QualitySettings.SetQualityLevel(qualityLevelIndex, applyExpensiveChanges: true);

        ValidateActiveTierPipeline(qualityLevelIndex);

        if (logOnSuccess)
        {
            FineageLog.Info(
                FineageLogCategory.World,
                LogPrefix + ": active tier " + names[qualityLevelIndex] + " (index " + qualityLevelIndex + ")");
        }
    }

    public static void ApplyBootGraphicsQuality() => ApplyQualityLevel(ResolveBootQualityIndex());

    public static void ValidateActiveTierPipeline(int qualityLevelIndex)
    {
        string tierName = QualitySettings.names[qualityLevelIndex];
        var activePipeline = QualitySettings.renderPipeline ?? GraphicsSettings.currentRenderPipeline;

        if (Catalog.TryGetTierPipeline(qualityLevelIndex, out GraphicsQualityCatalog.TierPipelineEntry expected)
            && expected.Pipeline != null)
        {
            if (activePipeline == null)
            {
                FineageLog.Error(
                    FineageLogCategory.World,
                    LogPrefix + ": tier '" + tierName + "' (index " + qualityLevelIndex +
                    ") has no active Render Pipeline; expected asset '" + expected.Pipeline.name + "'");
                return;
            }

            if (activePipeline != expected.Pipeline)
            {
                FineageLog.Error(
                    FineageLogCategory.World,
                    LogPrefix + ": tier '" + tierName + "' pipeline mismatch: active '" + activePipeline.name +
                    "' expected '" + expected.Pipeline.name + "'");
            }

            return;
        }

        if (activePipeline == null)
        {
            FineageLog.Error(
                FineageLogCategory.World,
                LogPrefix + ": tier '" + tierName + "' (index " + qualityLevelIndex +
                ") has no customRenderPipeline; assign a URP asset in QualitySettings for this tier");
        }
    }

    static GraphicsQualityCatalog LoadCatalog()
    {
        var loaded = Resources.Load<GraphicsQualityCatalog>(GraphicsQualityCatalog.DefaultResourcesPath);
        if (loaded != null)
            return loaded;

        FineageLog.Error(
            FineageLogCategory.World,
            LogPrefix + ": missing Resources asset at '" + GraphicsQualityCatalog.DefaultResourcesPath +
            "' — using in-memory defaults (tier pipeline validation disabled)");
        return ScriptableObject.CreateInstance<GraphicsQualityCatalog>();
    }

#if UNITY_EDITOR
    public static void ResetCatalogForTests() => _catalog = null;
#endif
}