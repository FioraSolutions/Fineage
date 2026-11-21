using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Profile table: platform default quality indices and optional per-tier pipeline references for validation.
/// Loaded from Resources at boot; see <see cref="GraphicsQualityApplier"/>.
/// </summary>
[CreateAssetMenu(fileName = "GraphicsQualityCatalog", menuName = "Fineage/Graphics Quality Catalog")]
public sealed class GraphicsQualityCatalog : ScriptableObject
{
    public const string DefaultResourcesPath = "Data/Settings/GraphicsQualityCatalog";

    [Serializable]
    public struct PlatformDefaultEntry
    {
        public GraphicsPlatformGroup PlatformGroup;
        [Tooltip("QualitySettings level index (Very Low=0 … Ultra=5).")]
        public int QualityLevelIndex;
    }

    [Serializable]
    public struct TierPipelineEntry
    {
        [Tooltip("Must match QualitySettings.names[index].")]
        public string TierName;
        public int QualityLevelIndex;
        public UniversalRenderPipelineAsset Pipeline;
    }

    [SerializeField] PlatformDefaultEntry[] platformDefaults =
    {
        new() { PlatformGroup = GraphicsPlatformGroup.Desktop, QualityLevelIndex = 3 },
        new() { PlatformGroup = GraphicsPlatformGroup.Mobile, QualityLevelIndex = 1 },
    };

    [SerializeField] TierPipelineEntry[] tierPipelines;

    public int ResolveDefaultQualityIndex(GraphicsPlatformGroup group)
    {
        if (platformDefaults != null)
        {
            for (int i = 0; i < platformDefaults.Length; i++)
            {
                if (platformDefaults[i].PlatformGroup == group)
                    return platformDefaults[i].QualityLevelIndex;
            }
        }

        return group == GraphicsPlatformGroup.Mobile ? 1 : 3;
    }

    public bool TryGetTierPipeline(int qualityLevelIndex, out TierPipelineEntry entry)
    {
        if (tierPipelines != null)
        {
            for (int i = 0; i < tierPipelines.Length; i++)
            {
                if (tierPipelines[i].QualityLevelIndex == qualityLevelIndex)
                {
                    entry = tierPipelines[i];
                    return true;
                }
            }
        }

        entry = default;
        return false;
    }

    public bool TierNamesMatchProjectQualitySettings()
    {
        string[] names = QualitySettings.names;
        if (tierPipelines == null)
            return false;

        for (int i = 0; i < tierPipelines.Length; i++)
        {
            int idx = tierPipelines[i].QualityLevelIndex;
            if (idx < 0 || idx >= names.Length)
                return false;
            if (!string.Equals(tierPipelines[i].TierName, names[idx], StringComparison.Ordinal))
                return false;
        }

        return true;
    }
}