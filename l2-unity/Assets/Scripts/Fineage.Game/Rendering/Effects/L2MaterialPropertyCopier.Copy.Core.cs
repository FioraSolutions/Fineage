using System;
using UnityEngine;

public static partial class L2MaterialPropertyCopier
{
    public static void CopyLifetimeFadeAndFxFromShared(Material runtimeMat, Material sharedMat)
    {
        if (runtimeMat == null || sharedMat == null)
        {
            return;
        }

        CopyFloatIfPresent(runtimeMat, sharedMat, HasLifetimeId);
        // CompositePrefabEffect may call EffectShaderLifetimeHelper.Apply(false) on the whole instance;
        // per-slot spawn must restore authored lifetime fade (upline SpriteEmitter2, etc.).
        RestoreShaderLifetimeFromSharedFadeAuthored(runtimeMat, sharedMat);
        CopyVectorIfPresent(runtimeMat, sharedMat, LifetimeRangeId);
        CopyVectorIfPresent(runtimeMat, sharedMat, InitialDelayRangeId);
        CopyFloatIfPresent(runtimeMat, sharedMat, LoopSizeScalePreviewId);
        CopyFloatIfPresent(runtimeMat, sharedMat, Shader.PropertyToID("_ColorFadeAlphaBlend"));
        CopyFloatIfPresent(runtimeMat, sharedMat, FadeInId);
        CopyFloatIfPresent(runtimeMat, sharedMat, FadeInEndTimeId);
        CopyFloatIfPresent(runtimeMat, sharedMat, FadeoutId);
        CopyFloatIfPresent(runtimeMat, sharedMat, FadeoutStartTimeId);
        CopyFloatIfPresent(runtimeMat, sharedMat, Shader.PropertyToID("_FadeOutStartTime"));
        CopyFloatIfPresent(runtimeMat, sharedMat, Shader.PropertyToID("_FadeOut"));
        CopyFloatIfPresent(runtimeMat, sharedMat, FadeOutPowerId);
        if (Application.isPlaying && runtimeMat.HasProperty(DebugAtlasPreviewId))
        {
            runtimeMat.SetFloat(DebugAtlasPreviewId, 0f);
        }
        else
        {
            CopyFloatIfPresent(runtimeMat, sharedMat, DebugAtlasPreviewId);
        }
        CopyFloatIfPresent(runtimeMat, sharedMat, Shader.PropertyToID("_L2FxWorldCalibration"));
        CopyFloatIfPresent(runtimeMat, sharedMat, Shader.PropertyToID("_L2FxHeUnitScaleEnable"));
        CopyFloatIfPresent(runtimeMat, sharedMat, Shader.PropertyToID("_L2FxHeUnitScale"));
        CopyFloatIfPresent(runtimeMat, sharedMat, Shader.PropertyToID("_VertMeshAnimEnable"));
        CopyFloatIfPresent(runtimeMat, sharedMat, Shader.PropertyToID("_VertMeshFrameCount"));
        CopyFloatIfPresent(runtimeMat, sharedMat, Shader.PropertyToID("_VertMeshSeqRate"));
        CopyFloatIfPresent(runtimeMat, sharedMat, Shader.PropertyToID("_ExpandShaderBounds"));
        if (runtimeMat.HasProperty("_VertMeshFramePosTex") && sharedMat.HasProperty("_VertMeshFramePosTex"))
        {
            runtimeMat.SetTexture("_VertMeshFramePosTex", sharedMat.GetTexture("_VertMeshFramePosTex"));
        }
        CopyFloatIfPresent(runtimeMat, sharedMat, RgbBoostId);
        CopyColorIfPresent(runtimeMat, sharedMat, TextureFactorId);
        CopyFloatIfPresent(runtimeMat, sharedMat, TextureContrastId);
        CopyFloatIfPresent(runtimeMat, sharedMat, TextureFloorId);
        CopyFloatIfPresent(runtimeMat, sharedMat, AlphaBoostId);
        CopyFloatIfPresent(runtimeMat, sharedMat, OpacityId);
        CopyFloatIfPresent(runtimeMat, sharedMat, AtlasUvRemapId);
        CopyVectorIfPresent(runtimeMat, sharedMat, AtlasUvMinMaxId);
        CopyFloatIfPresent(runtimeMat, sharedMat, IgnoreMainTexAlphaId);
        CopyFloatIfPresent(runtimeMat, sharedMat, EmitterAlphaId);
        CopyFloatIfPresent(runtimeMat, sharedMat, AlphaEdgeFeatherId);
        CopyFloatIfPresent(runtimeMat, sharedMat, SplitRibbonByLumId);
        CopyFloatIfPresent(runtimeMat, sharedMat, SoftLumMinId);
        CopyFloatIfPresent(runtimeMat, sharedMat, SoftLumMaxId);
        CopyFloatIfPresent(runtimeMat, sharedMat, LineLumMinId);
        CopyFloatIfPresent(runtimeMat, sharedMat, LineLumMaxId);
        CopyFloatIfPresent(runtimeMat, sharedMat, SoftOpacityMulId);
        CopyFloatIfPresent(runtimeMat, sharedMat, LineOpacityMulId);
        CopyFloatIfPresent(runtimeMat, sharedMat, SoftRgbBoostId);
        CopyFloatIfPresent(runtimeMat, sharedMat, LineRgbBoostId);
        CopyFloatIfPresent(runtimeMat, sharedMat, SplitByUvLayerId);
        CopyVectorIfPresent(runtimeMat, sharedMat, UvLayerCenterId);
        CopyFloatIfPresent(runtimeMat, sharedMat, UvLayerDistMinId);
        CopyFloatIfPresent(runtimeMat, sharedMat, UvLayerDistMaxId);
        CopyFloatIfPresent(runtimeMat, sharedMat, OuterSoftOpacityMulId);
        CopyFloatIfPresent(runtimeMat, sharedMat, OuterLineOpacityMulId);
        CopyFloatIfPresent(runtimeMat, sharedMat, OuterSoftRgbBoostId);
        CopyFloatIfPresent(runtimeMat, sharedMat, OuterLineRgbBoostId);
        CopyFloatIfPresent(runtimeMat, sharedMat, ColorScaleRepeatsId);
        CopyFloatIfPresent(runtimeMat, sharedMat, ColorScaleCountId);
        CopyFloatIfPresent(runtimeMat, sharedMat, ColorScaleTime1Id);
        CopyFloatIfPresent(runtimeMat, sharedMat, ColorScaleTime2Id);
        CopyColorIfPresent(runtimeMat, sharedMat, ColorScale0Id);
        CopyColorIfPresent(runtimeMat, sharedMat, ColorScale1Id);
        CopyColorIfPresent(runtimeMat, sharedMat, ColorScale2Id);
        CopyFloatIfPresent(runtimeMat, sharedMat, BAlphaBlendId);
        CopyFloatIfPresent(runtimeMat, sharedMat, BillboardToCameraId);
        CopyVectorIfPresent(runtimeMat, sharedMat, BillboardWorldUpId);
        CopyVectorIfPresent(runtimeMat, sharedMat, BillboardEulerOffsetId);
        CopyVectorIfPresent(runtimeMat, sharedMat, SpriteSpinStartRangeUcId);
        CopyVectorIfPresent(runtimeMat, sharedMat, SpriteSpinSpsRangeUcId);
        CopyVectorIfPresent(runtimeMat, sharedMat, SpriteSpinCcwOrCwId);
        CopyVectorIfPresent(runtimeMat, sharedMat, StartVelocityRangeXUcId);
        CopyVectorIfPresent(runtimeMat, sharedMat, StartVelocityRangeYUcId);
        CopyVectorIfPresent(runtimeMat, sharedMat, StartVelocityRangeZUcId);
    }

    private static void RestoreShaderLifetimeFromSharedFadeAuthored(Material runtimeMat, Material sharedMat)
    {
        if (runtimeMat == null || sharedMat == null || !runtimeMat.HasProperty(HasLifetimeId))
        {
            return;
        }

        float sharedHasLifetime = sharedMat.HasProperty(HasLifetimeId) ? sharedMat.GetFloat(HasLifetimeId) : 0f;
        float fadeOut = sharedMat.HasProperty(FadeoutId) ? sharedMat.GetFloat(FadeoutId) : 0f;
        if (sharedHasLifetime > 0.5f || fadeOut > 0.5f)
        {
            runtimeMat.SetFloat(HasLifetimeId, 1f);
        }
    }
}
