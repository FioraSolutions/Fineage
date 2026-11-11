using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public sealed partial class L2YebisPostRenderPass
{
    void ExecuteYebis(CommandBuffer cmd, RTHandle cameraColor, CameraData cameraData, RTHandle cameraDepth)
    {
        ExecuteYebis(
            cmd,
            cameraColor,
            cameraData.cameraType,
            cameraData.renderType,
            cameraData.cameraTargetDescriptor.graphicsFormat,
            cameraDepth);
    }

    void ExecuteYebis(CommandBuffer cmd, RTHandle cameraColor, UniversalCameraData cameraData, RTHandle cameraDepth)
    {
        ExecuteYebis(
            cmd,
            cameraColor,
            cameraData.cameraType,
            cameraData.renderType,
            cameraData.cameraTargetDescriptor.graphicsFormat,
            cameraDepth);
    }

    void ExecuteYebis(
        CommandBuffer cmd,
        RTHandle cameraColor,
        CameraType cameraType,
        CameraRenderType renderType,
        GraphicsFormat cameraFormat,
        RTHandle cameraDepth)
    {
            BindParams();

            _material.SetFloat(LumaGridId, 0f);
            cmd.SetGlobalFloat(LumaGridId, 0f);
            SetBlitTexelSize(cameraColor);
            BlitFull(cmd, cameraColor, _luma0, PassLuma);
            SetBlitTexelSize(_luma0);
            BlitFull(cmd, _luma0, _luma1, PassLuma);
            _material.SetFloat(LumaGridId, 8f);
            cmd.SetGlobalFloat(LumaGridId, 8f);
            SetBlitTexelSize(_luma1);
            BlitFull(cmd, _luma1, _luma2, PassLuma);
            _material.SetFloat(LumaGridId, 0f);
            cmd.SetGlobalFloat(LumaGridId, 0f);

            if (!_adaptedSeeded)
            {
                // Seed from this frame's 8x8 luma, not middleGray 0.18.
                // 0.18 + mapping 18 → gain 9 and a white flash (the old 2.1 workaround).
                BlitFull(cmd, _luma2, _adapted, PassCopy);
                BlitFull(cmd, _luma2, _adaptedPrev, PassCopy);
                _adaptedSeeded = true;
            }

            cmd.SetGlobalTexture(AdaptedTexId, _adaptedPrev.nameID);
            BlitFull(cmd, _luma2, _adapted, PassAdapt);
            BlitFull(cmd, _adapted, _adaptedPrev, PassCopy);

            cmd.SetGlobalTexture(AdaptedTexId, _adapted.nameID);
            cmd.SetGlobalTexture(BloomTexId, Texture2D.blackTexture);

            int debug = _settings.yebisDebugMode;
            // Off while the confirmed combine is being checked. Kawase was added after the curve.
            // Restore: wantBloom = intensity > 0.0001 && debug != 1. Saved intensity is yebisBloomIntensity (0.18).
            bool wantBloom = false;

            bool gameBase = cameraType == CameraType.Game
                && renderType == CameraRenderType.Base;
            string captureFolder = null;
            if (gameBase && L2YebisLiveCapture.Consume())
            {
                if (_captureScene == null || _captureScene.rt == null || _captureGrade == null || _captureGrade.rt == null || _captureControl == null || _captureControl.rt == null)
                {
                    // Debug.LogWarning("[Yebis] свои RT для снимка ещё не созданы");
                }
                else
                {
                    float hours = WorldClockPacketServices.Clock != null ? WorldClockPacketServices.Clock.WorldHours : -1f;
                    captureFolder = L2YebisLiveCapture.CreateFolder(
                        hours,
                        cameraFormat.ToString(),
                        QualitySettings.activeColorSpace.ToString());
                    cmd.SetRenderTarget(_captureControl.nameID);
                    cmd.ClearRenderTarget(false, true, new Color(0.8f, 0.2f, 0.05f, 1f));
                    cmd.SetRenderTarget(_captureScene.nameID);
                    cmd.ClearRenderTarget(false, true, new Color(1f, 0f, 1f, 1f));
                    cmd.SetRenderTarget(_captureGrade.nameID);
                    cmd.ClearRenderTarget(false, true, new Color(1f, 0f, 1f, 1f));
                    CopyCapture(cmd, cameraColor, _captureScene);
                }
            }

            // Curve 0A08B2C83DB6048D writes the half-float buffer combine samples.
            // The confirmed combine formula is unchanged.
            RTHandle combineSource = cameraColor;
            if (_sceneFloat != null)
            {
                BlitFull(cmd, cameraColor, _sceneFloat, PassSceneToFloat);
                combineSource = _sceneFloat;
            }

            // Debug 3+ reads the raw scene (luma/gain). Bloom extract is from the
            // graded look so mid-tone skills (blue particles) still pass.
            if (debug >= 3)
            {
                BlitFull(cmd, combineSource, _temp, PassCombine);
                if (captureFolder != null)
                    CopyCapture(cmd, _temp, _captureGrade);
                BlitFull(cmd, _temp, cameraColor, PassCopy);
            }
            else
            {
                BlitFull(cmd, combineSource, _temp, PassCombine);
                if (captureFolder != null)
                    CopyCapture(cmd, _temp, _captureGrade);

                if (wantBloom)
                {
                    if (_settings.yebisBloomExcludeSkyCloudsSun && cameraDepth != null)
                        FillBloomExcludeMask(cmd, cameraDepth);
                    else
                        BindBloomExclude(cmd, false);

                    SetBlitTexelSize(_temp);
                    BlitFull(cmd, _temp, _bloomA, PassExtract);
                    _material.SetFloat(KawaseOffsetId, 1f);
                    SetBlitTexelSize(_bloomA);
                    BlitFull(cmd, _bloomA, _bloomB, PassKawase);
                    _material.SetFloat(KawaseOffsetId, 2f);
                    SetBlitTexelSize(_bloomB);
                    BlitFull(cmd, _bloomB, _bloomA, PassKawase);
                    _material.SetFloat(KawaseOffsetId, 3f);
                    SetBlitTexelSize(_bloomA);
                    BlitFull(cmd, _bloomA, _bloomB, PassKawase);
                    cmd.SetGlobalTexture(BloomTexId, _bloomB.nameID);
                    if (debug == 2)
                        BlitFull(cmd, _bloomB, cameraColor, PassCopy);
                    else
                        BlitFull(cmd, _temp, cameraColor, PassAddBloom);
                }
                else
                {
                    BlitFull(cmd, _temp, cameraColor, PassCopy);
                }
            }

            CoreUtils.SetRenderTarget(cmd, cameraColor, ClearFlag.None, Color.clear);
            if (captureFolder != null && _captureScene != null && _captureScene.rt != null && _captureGrade != null && _captureGrade.rt != null && _captureControl != null && _captureControl.rt != null)
            {
                // Debug.Log("[Yebis] ReadAfterFrame после записи своих RT, не временных целей URP");
                L2YebisLiveCapture.ReadAfterFrame(captureFolder, _captureScene.rt, _captureGrade.rt, _captureControl.rt);
            }
            L2NameplateOverlayQueue.Flush(cmd);
            MaybeLogYebis(cmd, cameraColor);
    }

    void BindParams()
    {
        L2FxCompositorSettings s = _settings;
        _material.SetFloat(BloomThresholdId, s.yebisBloomThreshold);
        _material.SetFloat(BloomRemapId, s.yebisBloomRemap);
        _material.SetFloat(BloomIntensityId, s.yebisBloomIntensity);
        _material.SetFloat(AdaptLerpId, Mathf.Clamp01(s.yebisAdaptationSensitivity * 0.16f));
        _material.SetFloat(YebisDebugId, s.yebisDebugMode);
        _material.SetFloat(ExposureId, s.yebisExposure);
        _material.SetFloat(UseExposureId, s.yebisUseExposure);
        _material.SetFloat(Mapping0Id, s.yebisMappingFactor0);
        _material.SetFloat(MiddleGrayId, s.yebisMiddleGray);
        _material.SetFloat(AdaptationScaleId, s.yebisAdaptationScale);
        _material.SetFloat(PrePostLumaScaleId, s.yebisPrePostLumaScale);
        _material.SetFloat(AdaptedOverrideId, s.yebisAdaptedOverride);
        _material.SetFloat(ColorGammaId, s.yebisColorGamma);
        _material.SetFloat(ColorSaturationId, s.yebisColorSaturation);
        _material.SetFloat(FilmicAId, s.yebisFilmicA);
        _material.SetFloat(FilmicBId, s.yebisFilmicB);
        _material.SetFloat(MatrixBiasId, s.yebisMatrixBias);
        _material.SetFloat(GainMinId, s.yebisGainMin);
        _material.SetFloat(GainMaxId, s.yebisGainMax);
        _material.SetFloat(ContrastMixId, s.yebisContrastMix);
        _material.SetFloat(Pre3898Id, s.yebisPre3898);
        _material.SetFloat(CombineGainId, s.yebisCombineGain);
        _material.SetFloat(FirstMatrixId, s.yebisFirstMatrix);
        _material.SetFloat(SecondMatrixId, s.yebisSecondMatrix);
        _material.SetFloat(DisplayGammaId, s.yebisDisplayGamma);
        // Confirmed combine. 0 would restore L2Yebis_GradeLegacy.
        _material.SetFloat(CaptureFormulaId, 1f);
        // 0 full grade. 1 sample, 2 matrix, 3 before power, 4 after power. F7 cycles these.
        _material.SetFloat(GradeStageId, L2YebisLiveCapture.GradeStage);
    }
}
