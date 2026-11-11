using UnityEngine;

public static partial class ShaderFadeDiagnostic
{
    public static string BuildLine(Material mat, float now)
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (mat == null)
        {
            return "null_mat";
        }

        string sn = mat.shader != null ? mat.shader.name : "no_shader";
        float hasLt = mat.HasProperty(HasLifetimeShaderId) ? mat.GetFloat(HasLifetimeShaderId) : -1f;
        float fadeout = mat.HasProperty(FadeoutShaderId) ? mat.GetFloat(FadeoutShaderId) : -1f;
        float fadeIn = mat.HasProperty(FadeInShaderId) ? mat.GetFloat(FadeInShaderId) : -1f;
        float fadeInEnd = mat.HasProperty(FadeInEndTimeShaderId) ? mat.GetFloat(FadeInEndTimeShaderId) : -1f;
        float fadeStart = mat.HasProperty(FadeoutStartTimeShaderId) ? mat.GetFloat(FadeoutStartTimeShaderId) : -1f;
        Vector4 life = mat.HasProperty(LifetimeRangeShaderId) ? mat.GetVector(LifetimeRangeShaderId) : Vector4.zero;
        float lifeMax = Mathf.Max(life.x, life.y, 1e-6f);
        float hold = mat.HasProperty(HoldShaderId) ? mat.GetFloat(HoldShaderId) : -1f;
        float st = mat.HasProperty(StartTimeShaderId) ? mat.GetFloat(StartTimeShaderId) : -1f;
        float age = st > -0.5f ? now - st : -1f;
        Vector4 idr = mat.HasProperty(InitialDelayRangeShaderId) ? mat.GetVector(InitialDelayRangeShaderId) : Vector4.zero;
        float delayMid = (Mathf.Min(idr.x, idr.y) + Mathf.Max(idr.x, idr.y)) * 0.5f;
        float ageVisMid = age >= 0f ? age - delayMid : age;
        float holdAnimAge = (hold > 1e-4f && ageVisMid >= 0f) ? Mathf.Min(ageVisMid, lifeMax * hold) : -1f;
        float debugPreview = mat.HasProperty(DebugMeshPreviewShaderId) ? mat.GetFloat(DebugMeshPreviewShaderId) : -1f;
        float debugLoop = mat.HasProperty(DebugMeshPreviewLoopShaderId) ? mat.GetFloat(DebugMeshPreviewLoopShaderId) : -1f;
        float opacity = mat.HasProperty(OpacityShaderId) ? mat.GetFloat(OpacityShaderId) : -1f;
        float emitterAlpha = mat.HasProperty(EmitterAlphaShaderId) ? mat.GetFloat(EmitterAlphaShaderId) : -1f;
        float loopAge = (hasLt < 0.5f && age >= 0f) ? Mathf.Repeat(age, lifeMax) : -1f;
        float fadeAge = holdAnimAge >= 0f ? holdAnimAge : (loopAge >= 0f ? loopAge : ageVisMid >= 0f ? ageVisMid : age);
        float tail = (fadeStart >= 0f && lifeMax > 1e-4f) ? lifeMax - fadeStart : -1f;
        float fadeFrac =
            tail > 1e-4f && fadeAge >= fadeStart
                ? Mathf.Clamp01((fadeAge - fadeStart) / tail)
                : -1f;
        return
            $"{mat.name} shader={sn} HasLt={hasLt} FadeInOn={fadeIn} FadeInEnd={fadeInEnd:F4} " +
            $"FadeoutOn={fadeout} fadeOutStart={fadeStart:F4} lifeMax={lifeMax:F4} tail={tail:F4} Hold={hold} " +
            $"initDelay=({idr.x:F3},{idr.y:F3}) StartT={st:F4} age={age:F4}s holdAnimAge={(holdAnimAge >= 0f ? holdAnimAge.ToString("F4") : "-1")} loopAge={loopAge:F4}s " +
            $"DebugPreview={debugPreview:F0} DebugLoop={debugLoop:F0} Opacity={opacity:F3} EmitterA={emitterAlpha:F3} " +
            $"fadeOutFrac~={fadeFrac:F3}";
#else
        return string.Empty;
#endif
    }
}
