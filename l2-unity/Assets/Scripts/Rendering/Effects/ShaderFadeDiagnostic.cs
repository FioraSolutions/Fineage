using UnityEngine;

public static partial class ShaderFadeDiagnostic
{
    private static readonly int HasLifetimeShaderId = Shader.PropertyToID("_HasLifetime");
    private static readonly int HoldShaderId = Shader.PropertyToID("_Hold");
    private static readonly int FadeInShaderId = Shader.PropertyToID("_FadeIn");
    private static readonly int FadeInEndTimeShaderId = Shader.PropertyToID("_FadeInEndTime");
    private static readonly int FadeoutShaderId = Shader.PropertyToID("_Fadeout");
    private static readonly int FadeoutStartTimeShaderId = Shader.PropertyToID("_FadeoutStartTime");
    private static readonly int LifetimeRangeShaderId = Shader.PropertyToID("_LifetimeRange");
    private static readonly int StartTimeShaderId = Shader.PropertyToID("_StartTime");
    private static readonly int InitialDelayRangeShaderId = Shader.PropertyToID("_InitialDelayRange");
    private static readonly int DebugMeshPreviewShaderId = Shader.PropertyToID("_DebugMeshPreview");
    private static readonly int DebugMeshPreviewLoopShaderId = Shader.PropertyToID("_DebugMeshPreviewLoop");
    private static readonly int OpacityShaderId = Shader.PropertyToID("_Opacity");
    private static readonly int EmitterAlphaShaderId = Shader.PropertyToID("_EmitterAlpha");


    /// <summary>
    /// Грубая метка фазы по числам в материале (шейдер может отличаться). Ищите в Console: [FADE_PHASE].
    /// </summary>

}
