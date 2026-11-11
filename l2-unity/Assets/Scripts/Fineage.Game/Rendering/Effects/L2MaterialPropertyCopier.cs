using System;
using UnityEngine;

/// <summary>
/// GPU bridge: copies shared material params into runtime instances and writes dynamic shader uniforms.
/// </summary>
public static partial class L2MaterialPropertyCopier
{
    public const string OwnerWorldPosProperty = "_OwnerWorldPos";
    public const string UseExternalTargetPositionProperty = "_UseExternalTargetPosition";
    public const string UseOwnerFromShaderTargetProperty = "_UseOwnerFromShaderTarget";
    public const string L2FxTargetWorldPosProperty = "_L2FxTargetWorldPos";

    public static readonly int HoldId = Shader.PropertyToID("_Hold");
    public static readonly int HoldSizeReferenceId = Shader.PropertyToID("_HoldSizeReference");
    public static readonly int LifetimeRangeId = Shader.PropertyToID("_LifetimeRange");
    public static readonly int StartTimeId = Shader.PropertyToID("_StartTime");
    public static readonly int SeedId = Shader.PropertyToID("_Seed");
    public static readonly int HasLifetimeId = Shader.PropertyToID("_HasLifetime");
    public static readonly int InitialDelayRangeId = Shader.PropertyToID("_InitialDelayRange");
    public static readonly int LoopSizeScalePreviewId = Shader.PropertyToID("_LoopSizeScalePreview");
    public static readonly int FadeInId = Shader.PropertyToID("_FadeIn");
    public static readonly int FadeInEndTimeId = Shader.PropertyToID("_FadeInEndTime");
    public static readonly int FadeoutId = Shader.PropertyToID("_Fadeout");
    public static readonly int FadeoutStartTimeId = Shader.PropertyToID("_FadeoutStartTime");
    public static readonly int FadeOutPowerId = Shader.PropertyToID("_FadeOutPower");
    public static readonly int DebugAtlasPreviewId = Shader.PropertyToID("_DebugAtlasPreview");
    public static readonly int RgbBoostId = Shader.PropertyToID("_RgbBoost");
    public static readonly int TextureFactorId = Shader.PropertyToID("_TextureFactor");
    public static readonly int TextureContrastId = Shader.PropertyToID("_TextureContrast");
    public static readonly int TextureFloorId = Shader.PropertyToID("_TextureFloor");
    public static readonly int AlphaBoostId = Shader.PropertyToID("_AlphaBoost");
    public static readonly int OpacityId = Shader.PropertyToID("_Opacity");
    public static readonly int AtlasUvRemapId = Shader.PropertyToID("_AtlasUvRemap");
    public static readonly int AtlasUvMinMaxId = Shader.PropertyToID("_AtlasUvMinMax");
    public static readonly int IgnoreMainTexAlphaId = Shader.PropertyToID("_IgnoreMainTexAlpha");
    public static readonly int EmitterAlphaId = Shader.PropertyToID("_EmitterAlpha");
    public static readonly int AlphaEdgeFeatherId = Shader.PropertyToID("_AlphaEdgeFeather");
    public static readonly int SplitRibbonByLumId = Shader.PropertyToID("_SplitRibbonByLum");
    public static readonly int SoftLumMinId = Shader.PropertyToID("_SoftLumMin");
    public static readonly int SoftLumMaxId = Shader.PropertyToID("_SoftLumMax");
    public static readonly int LineLumMinId = Shader.PropertyToID("_LineLumMin");
    public static readonly int LineLumMaxId = Shader.PropertyToID("_LineLumMax");
    public static readonly int SoftOpacityMulId = Shader.PropertyToID("_SoftOpacityMul");
    public static readonly int LineOpacityMulId = Shader.PropertyToID("_LineOpacityMul");
    public static readonly int SoftRgbBoostId = Shader.PropertyToID("_SoftRgbBoost");
    public static readonly int LineRgbBoostId = Shader.PropertyToID("_LineRgbBoost");
    public static readonly int SplitByUvLayerId = Shader.PropertyToID("_SplitByUvLayer");
    public static readonly int UvLayerCenterId = Shader.PropertyToID("_UvLayerCenter");
    public static readonly int UvLayerDistMinId = Shader.PropertyToID("_UvLayerDistMin");
    public static readonly int UvLayerDistMaxId = Shader.PropertyToID("_UvLayerDistMax");
    public static readonly int OuterSoftOpacityMulId = Shader.PropertyToID("_OuterSoftOpacityMul");
    public static readonly int OuterLineOpacityMulId = Shader.PropertyToID("_OuterLineOpacityMul");
    public static readonly int OuterSoftRgbBoostId = Shader.PropertyToID("_OuterSoftRgbBoost");
    public static readonly int OuterLineRgbBoostId = Shader.PropertyToID("_OuterLineRgbBoost");
    public static readonly int ColorScaleRepeatsId = Shader.PropertyToID("_ColorScaleRepeats");
    public static readonly int ColorScaleCountId = Shader.PropertyToID("_ColorScaleCount");
    public static readonly int ColorScaleTime1Id = Shader.PropertyToID("_ColorScaleTime1");
    public static readonly int ColorScaleTime2Id = Shader.PropertyToID("_ColorScaleTime2");
    public static readonly int ColorScale0Id = Shader.PropertyToID("_ColorScale0");
    public static readonly int ColorScale1Id = Shader.PropertyToID("_ColorScale1");
    public static readonly int ColorScale2Id = Shader.PropertyToID("_ColorScale2");
    public static readonly int BAlphaBlendId = Shader.PropertyToID("_bAlphaBlend");
    public static readonly int BillboardToCameraId = Shader.PropertyToID("_BillboardToCamera");
    public static readonly int BillboardWorldUpId = Shader.PropertyToID("_BillboardWorldUp");
    public static readonly int BillboardEulerOffsetId = Shader.PropertyToID("_BillboardEulerOffset");
    public static readonly int UseExternalTargetPositionId = Shader.PropertyToID(UseExternalTargetPositionProperty);
    public static readonly int UseOwnerFromShaderTargetId = Shader.PropertyToID(UseOwnerFromShaderTargetProperty);
    public static readonly int L2FxTargetWorldPosId = Shader.PropertyToID(L2FxTargetWorldPosProperty);
    public static readonly int StartSpinRandStateBitsId = Shader.PropertyToID("_StartSpinRandStateBits");
    public static readonly int MeshSpawnRandStateBitsId = Shader.PropertyToID("_MeshSpawnRandStateBits");
    public static readonly int SpawnModeId = Shader.PropertyToID("_SpawnMode");
    public static readonly int SpinParticlesId = Shader.PropertyToID("_SpinParticles");
    public static readonly int StartSpinYawRangeUcId = Shader.PropertyToID("_StartSpinYawRangeUc");
    public static readonly int StartSpinPitchRangeUcId = Shader.PropertyToID("_StartSpinPitchRangeUc");
    public static readonly int StartSpinRollRangeUcId = Shader.PropertyToID("_StartSpinRollRangeUc");
    public static readonly int StartLocationZRangeUUId = Shader.PropertyToID("_StartLocationZRangeUU");
    public static readonly int StartVelocityZRangeUUId = Shader.PropertyToID("_StartVelocityZRangeUU");
    public static readonly int StartSizeZRangeId = Shader.PropertyToID("_StartSizeZRange");
    public static readonly int StartSizeXYId = Shader.PropertyToID("_StartSizeXY");
    public static readonly int ColorMultiplierId = Shader.PropertyToID("_ColorMultiplier");
    public static readonly int SpriteSpinRandStateBitsId = Shader.PropertyToID("_SpriteSpinRandStateBits");
    public static readonly int SpriteSpinStartRangeUcId = Shader.PropertyToID("_SpriteSpinStartRangeUc");
    public static readonly int SpriteSpinSpsRangeUcId = Shader.PropertyToID("_SpriteSpinSpsRangeUc");
    public static readonly int SpriteSpinCcwOrCwId = Shader.PropertyToID("_SpriteSpinCcwOrCw");
    public static readonly int SpriteMotionRandStateBitsId = Shader.PropertyToID("_SpriteMotionRandStateBits");
    public static readonly int SpriteSpinModeId = Shader.PropertyToID("_SpinMode");
    public static readonly int StartVelocityRangeXUcId = Shader.PropertyToID("_StartVelocityRangeXUc");
    public static readonly int StartVelocityRangeYUcId = Shader.PropertyToID("_StartVelocityRangeYUc");
    public static readonly int StartVelocityRangeZUcId = Shader.PropertyToID("_StartVelocityRangeZUc");
    public static readonly int StartLocationOffsetUeId = Shader.PropertyToID("_StartLocationOffsetUe");
    public static readonly int StartLocationRangeXUcId = Shader.PropertyToID("_StartLocationRangeXUc");
    public static readonly int StartLocationRangeYUcId = Shader.PropertyToID("_StartLocationRangeYUc");
    public static readonly int StartLocationRangeZUcId = Shader.PropertyToID("_StartLocationRangeZUc");
    public static readonly int StartSizeRangeXUcId = Shader.PropertyToID("_StartSizeRangeXUc");
    public static readonly int StartSizeRangeYUcId = Shader.PropertyToID("_StartSizeRangeYUc");
    public static readonly int StartSizeRangeZUcId = Shader.PropertyToID("_StartSizeRangeZUc");
    public static readonly int SpsYawRangeUcId = Shader.PropertyToID("_SpsYawRangeUc");
    public static readonly int SpsPitchRangeUcId = Shader.PropertyToID("_SpsPitchRangeUc");
    public static readonly int SpsRollRangeUcId = Shader.PropertyToID("_SpsRollRangeUc");
    public static readonly int SpinCCWorCWId = Shader.PropertyToID("_SpinCCWorCW");
    public static readonly int L2MotionReplayEnabledId = Shader.PropertyToID("_L2MotionReplayEnabled");
    public static readonly int SpawnDeltaTimeId = Shader.PropertyToID("_SpawnDeltaTime");

    private const uint AppRandMultiplier = 214013u;
    private const uint AppRandIncrement = 2531011u;
    // SpawnParticleSnapshot.log: m_u004_b / SpriteEmitter0, emitter 15475F00, slot 0.
    private const uint HealingPotionSe0ReplaySpawnState = 0x6FEC3FC2u;
    private const float HealingPotionSe0ReplaySpawnDeltaTime = 0.0111764f;
    // m_u004_b / MeshEmitter3: captured L2 SpawnParticle trace shows 31
    // appRand draws from slot 0's state before Roll to slot 1's state before Roll.
    // This is emitter-specific, not a general StartSpin rule.
    public const int MeshEmitter3SlotToSlotDrawCount = 31;
    // MeshEmitter SpawnParticle Loc/Vel/Size stream (L2FxMeshSpawnParticle).
    // LIVE VERIFIED: it_healing_potion_ta Name="Wave" (LocZ/VelZ path).
    // LIVE VERIFIED: shot_N_atk / e_u505 MeshEmitter225 "Spirit" (full XYZ),
    //   SpawnSoulShotSpiritCapture 2026-07-22 draws=28 scopes=12.
    // Base state is BEFORE StartVelocity. StartSpin = +22.
    public const int MeshSpawnSlotToSlotDrawCount = 31;
    public const int MeshSpawnDrawsBeforeStartSpin = 22;

    /// <summary>
    /// Opt-in: material exposes MeshSpawn TLS (_MeshSpawnRandStateBits).
    /// LIVE: Wave (LocZ/VelZ) + Spirit MeshEmitter225 (full XYZ).
    /// </summary>
}
