using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Global High Elf Yebis after sky/world/skills. Contrast is primary, bloom secondary.
/// Not gated on skills: WindStrike has no PostEffectDataID.
/// </summary>
public sealed partial class L2YebisPostRenderPass : ScriptableRenderPass
{
    const int PassExtract = 0;
    const int PassKawase = 1;
    const int PassLuma = 2;
    const int PassAdapt = 3;
    const int PassCombine = 4;
    const int PassCopy = 5;
    const int PassAddBloom = 6;
    const int PassSceneToFloat = 7;

    static readonly int BloomTexId = Shader.PropertyToID("_BloomTex");
    static readonly int AdaptedTexId = Shader.PropertyToID("_AdaptedTex");
    static readonly int BloomThresholdId = Shader.PropertyToID("_BloomThreshold");
    static readonly int BloomRemapId = Shader.PropertyToID("_BloomRemap");
    static readonly int BloomIntensityId = Shader.PropertyToID("_BloomIntensity");
    static readonly int KawaseOffsetId = Shader.PropertyToID("_KawaseOffset");
    static readonly int KawaseTexelSizeId = Shader.PropertyToID("_KawaseTexelSize");
    static readonly int AdaptLerpId = Shader.PropertyToID("_AdaptLerp");
    static readonly int YebisDebugId = Shader.PropertyToID("_YebisDebug");
    static readonly int ExposureId = Shader.PropertyToID("_Exposure");
    static readonly int UseExposureId = Shader.PropertyToID("_UseExposure");
    static readonly int Mapping0Id = Shader.PropertyToID("_Mapping0");
    static readonly int MiddleGrayId = Shader.PropertyToID("_MiddleGray");
    static readonly int AdaptationScaleId = Shader.PropertyToID("_AdaptationScale");
    static readonly int PrePostLumaScaleId = Shader.PropertyToID("_PrePostLumaScale");
    static readonly int AdaptedOverrideId = Shader.PropertyToID("_AdaptedOverride");
    static readonly int ColorGammaId = Shader.PropertyToID("_ColorGamma");
    static readonly int ColorSaturationId = Shader.PropertyToID("_ColorSaturation");
    static readonly int FilmicAId = Shader.PropertyToID("_FilmicA");
    static readonly int FilmicBId = Shader.PropertyToID("_FilmicB");
    static readonly int MatrixBiasId = Shader.PropertyToID("_MatrixBias");
    static readonly int GainMinId = Shader.PropertyToID("_GainMin");
    static readonly int GainMaxId = Shader.PropertyToID("_GainMax");
    static readonly int ContrastMixId = Shader.PropertyToID("_ContrastMix");
    static readonly int Pre3898Id = Shader.PropertyToID("_Pre3898");
    static readonly int CombineGainId = Shader.PropertyToID("_CombineGain");
    static readonly int FirstMatrixId = Shader.PropertyToID("_FirstMatrix");
    static readonly int SecondMatrixId = Shader.PropertyToID("_SecondMatrix");
    static readonly int DisplayGammaId = Shader.PropertyToID("_DisplayGamma");
    static readonly int CaptureFormulaId = Shader.PropertyToID("_CaptureFormula");
    static readonly int GradeStageId = Shader.PropertyToID("_GradeStage");
    static readonly int LumaGridId = Shader.PropertyToID("_LumaGrid");
    static readonly int BlitScaleBiasId = Shader.PropertyToID("_BlitScaleBias");
    static readonly int BlitTextureId = Shader.PropertyToID("_BlitTexture");
    static readonly int BloomExcludeId = Shader.PropertyToID("_BloomExclude");
    static readonly int BloomExcludeMaskId = Shader.PropertyToID("_BloomExcludeMask");
    static readonly ProfilingSampler Sampler = new ProfilingSampler("L2 Yebis Post");

    readonly L2FxCompositorSettings _settings;
    readonly Material _material;

    RTHandle _temp;
    RTHandle _bloomA;
    RTHandle _bloomB;
    RTHandle _luma0;
    RTHandle _luma1;
    RTHandle _luma2;
    RTHandle _adapted;
    RTHandle _adaptedPrev;
    RTHandle _sceneFloat;
    RTHandle _bloomExclude;
    RTHandle _captureScene;
    RTHandle _captureGrade;
    RTHandle _captureControl;
    Material _excludeSkyMaterial;
    bool _adaptedSeeded;
    int _execFrame;

    sealed class YebisPassData
    {
        public TextureHandle color;
        public TextureHandle depth;
        public bool hasDepth;
        public L2YebisPostRenderPass pass;
        public UniversalCameraData cameraData;
    }
}
