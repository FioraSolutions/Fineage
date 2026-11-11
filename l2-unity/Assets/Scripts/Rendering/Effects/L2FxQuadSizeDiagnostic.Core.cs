using UnityEngine;

public static partial class L2FxQuadSizeDiagnostic
{
    public static bool ShouldTrace(string groupName, L2Particle owner, Transform groupTransform)
    {
        if (string.IsNullOrEmpty(groupName))
        {
            return false;
        }

        bool isTargetGroup =
            groupName.IndexOf("SpriteEmitter0", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
            groupName.IndexOf("SpriteEmitter2", System.StringComparison.OrdinalIgnoreCase) >= 0;
        if (!isTargetGroup)
        {
            return false;
        }

        return false;
    }

    public static QuadSizeSnapshot Compute(string groupName, Renderer renderer, float now, Material runtimeMat = null)
    {
        var snap = new QuadSizeSnapshot();
        if (renderer == null)
        {
            return snap;
        }

        Material mat = runtimeMat;
        if (mat == null)
        {
            Material[] mats = renderer.materials;
            mat = (mats != null && mats.Length > 0) ? mats[0] : null;
        }

        if (mat == null || !mat.HasProperty("_SizeRange"))
        {
            return snap;
        }

        Vector4 sizeRange = mat.GetVector("_SizeRange");
        snap.startSizeMidUU = (sizeRange.x + sizeRange.y) * 0.5f;
        snap.worldCalibrationK = mat.HasProperty("_L2FxWorldCalibration")
            ? Mathf.Max(mat.GetFloat("_L2FxWorldCalibration"), 1e-4f)
            : DefaultWorldCalibration;

        Transform quadTransform = renderer.transform;
        snap.lossyScale = quadTransform.lossyScale;
        snap.quadSpan = Vector3.one;
        MeshFilter mf = renderer.GetComponent<MeshFilter>();
        if (mf != null && mf.sharedMesh != null)
        {
            snap.quadSpan = mf.sharedMesh.bounds.size;
        }

        bool manualTestAge = mat.HasProperty("_TestSizeScaleAge");
        if (manualTestAge)
        {
            snap.sizeScalePath = "SE0_EvaluateDynamicSizeScale";
            snap.ageNorm = mat.GetFloat("_TestSizeScaleAge");
            snap.sizeMul = EvaluateDynamicSizeScaleSe0(mat, snap.ageNorm, out snap.scaleT);
        }
        else
        {
            snap.sizeScalePath = "L2Fx_SpriteSizeScale";
            snap.initialDelaySec = ResolveInitialDelay(mat);
            snap.lifetimeSec = ResolveLifetime(mat);
            snap.ageSec = ResolveAgeSeconds(now, mat, snap.initialDelaySec);
            snap.ageNorm = ResolveParticleAgeNorm(now, mat, snap.ageSec, snap.lifetimeSec);
            snap.sizeMul = SampleSpriteSizeScaleFromUniforms(mat, snap.ageNorm, out snap.scaleT);
        }

        if (mat.HasProperty("_TestDisableSizeScale") && mat.GetFloat("_TestDisableSizeScale") > 0.5f)
        {
            snap.sizeMul = 1f;
            snap.sizeScalePath = "TEST_SizeScaleDisabled";
        }

        snap.sizeUU = snap.startSizeMidUU * snap.sizeMul;
        float heUnit = 1f;
        if (mat.HasProperty("_L2FxHeUnitScaleEnable") &&
            mat.GetFloat("_L2FxHeUnitScaleEnable") > 0.5f &&
            mat.HasProperty("_L2FxHeUnitScale"))
        {
            heUnit = Mathf.Max(mat.GetFloat("_L2FxHeUnitScale"), 1e-6f);
        }

        float sizeInMeters = snap.sizeUU * heUnit * UuToMeters;
        snap.sizeMetersVertex = sizeInMeters * snap.worldCalibrationK;
        float worldDiameterM = sizeInMeters * snap.worldCalibrationK;
        snap.finalWidthM = snap.quadSpan.x * worldDiameterM * Mathf.Abs(snap.lossyScale.x);
        snap.finalHeightM = snap.quadSpan.y * worldDiameterM * Mathf.Abs(snap.lossyScale.y);
        snap.loopPreview = mat.HasProperty("_LoopSizeScalePreview") &&
            mat.GetFloat("_LoopSizeScalePreview") > 0.5f;
        ResolveFlipbookFrames(mat, snap.ageNorm, out snap.flipbookFrameA, out snap.flipbookFrameB, out snap.flipbookBlend);
        return snap;
    }

    public static void Log(string groupName, Renderer renderer, float now, Material runtimeMat = null)
    {
        Material mat = runtimeMat;
        if (mat == null && renderer != null)
        {
            Material[] mats = renderer.materials;
            mat = (mats != null && mats.Length > 0) ? mats[0] : null;
        }

        if (mat == null)
        {
            return;
        }

        QuadSizeSnapshot s = Compute(groupName, renderer, now, mat);
        if (s.startSizeMidUU <= 0f)
        {
            return;
        }

        float startTime = mat.HasProperty("_StartTime") ? mat.GetFloat("_StartTime") : 0f;
        float seed = mat.HasProperty("_Seed") ? mat.GetFloat("_Seed") : 0f;
        float repeats = mat.HasProperty("_SizeScaleRepeats") ? mat.GetFloat("_SizeScaleRepeats") : 0f;

        // Debug.Log(
            // $"[L2FX_QUAD_SIZE] group='{groupName}' mat='{mat.name}' now={now:F3}s frame={Time.frameCount}\n" +
            // $"  path={s.sizeScalePath}  ageNorm={s.ageNorm:F4}  scaleT=frac((1+repeats)*age)={s.scaleT:F4}  repeats={repeats:F2}\n" +
            // $"  runtime: _StartTime={startTime:F3} _Seed={seed:F3} delay={s.initialDelaySec:F4}s lifetime={s.lifetimeSec:F4}s ageSec={s.ageSec:F4}s\n" +
            // $"  StartSize midUU={s.startSizeMidUU:F4}  sizeMul={s.sizeMul:F4}  sizeUU={s.sizeUU:F4}\n" +
            // $"  vertex OS: sizeM=sizeUU/52.5*K = {s.sizeMetersVertex:F6}m  K={s.worldCalibrationK:F3}\n" +
            // $"  world: quadSpan=({s.quadSpan.x:F3},{s.quadSpan.y:F3}) lossyScale=({s.lossyScale.x:F4},{s.lossyScale.y:F4})\n" +
            // $"  FINAL diameter(m): width={s.finalWidthM:F6}  height={s.finalHeightM:F6}\n" +
            // $"  previewLoop={s.loopPreview} flipbook=frame{s.flipbookFrameA}->frame{s.flipbookFrameB} blend={s.flipbookBlend:F4}\n" +
            // $"  formula: finalWidth = midUU * sizeMul / 52.5 * K * quadSpan.x * abs(lossyScale.x)");
    }
}
