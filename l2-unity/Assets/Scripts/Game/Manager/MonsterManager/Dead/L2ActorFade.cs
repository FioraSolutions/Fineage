using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Unity stand-in for AActor::SetAlphaTexModifier(uchar).
/// Instances the current body/armor shader and drives _ActorAlpha.
/// Does not swap onto Lit or L2/Actor/Fade.
/// </summary>
public sealed partial class L2ActorFade
{
    public const float DurationSeconds = 2f;
    public const float AlphaPerSecond = 127.5f;
    public const byte AppearStartAlpha = 1;
    const string FadeKeyword = "_ACTOR_FADE";

    static readonly int ActorAlphaId = Shader.PropertyToID("_ActorAlpha");
    static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    static readonly int SrcBlendId = Shader.PropertyToID("_SrcBlend");
    static readonly int DstBlendId = Shader.PropertyToID("_DstBlend");
    static readonly int ZWriteId = Shader.PropertyToID("_ZWrite");

    /// <summary>Death / logout fade-out: 255 → 0 over DurationSeconds.</summary>
}
