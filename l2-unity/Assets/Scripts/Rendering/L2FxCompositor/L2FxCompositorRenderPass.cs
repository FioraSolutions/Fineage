using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Draws SkillEffect FX after world transparents. In direct mode the camera
/// target is already raw UNORM, so effects draw into cameraColor without copies.
/// The legacy encode/post/decode path remains available as a fallback.
/// </summary>
public sealed partial class L2FxCompositorRenderPass : ScriptableRenderPass
{
    const int PostExtractPass = 0;
    const int PostKawasePass = 1;
    const int PostCompositePass = 2;
    const int PostDebugPass = 3;
    const int TransferEncodeSkyPass = 2;

    static readonly int D3D9ActiveId = Shader.PropertyToID("_L2FxD3D9CompositorActive");
    static readonly int DebugModeId = Shader.PropertyToID("_L2FxCompositorDebugMode");
    static readonly int SceneTexId = Shader.PropertyToID("_SceneTex");
    static readonly int BloomTexId = Shader.PropertyToID("_BloomTex");
    static readonly int BloomThresholdId = Shader.PropertyToID("_BloomThreshold");
    static readonly int BloomPreGainId = Shader.PropertyToID("_BloomPreGain");
    static readonly int BloomIntensityId = Shader.PropertyToID("_BloomIntensity");
    static readonly int FxGainId = Shader.PropertyToID("_FxGain");
    static readonly int KawaseOffsetId = Shader.PropertyToID("_KawaseOffset");
    static readonly int KawaseTexelSizeId = Shader.PropertyToID("_KawaseTexelSize");
    static readonly ShaderTagId ForwardOnlyTag = new ShaderTagId("UniversalForwardOnly");
    static readonly ProfilingSampler Sampler = new ProfilingSampler("L2Fx D3D9 Compositor");
    static readonly ProfilingSampler SamplerDirect = new ProfilingSampler("L2 FX Direct UNORM");

    readonly L2FxCompositorSettings _settings;
    readonly Material _transferMaterial;
    readonly Material _postMaterial;

    RTHandle _l2Color;
    RTHandle _l2Scene;
    RTHandle _l2Sky;
    RTHandle _l2BloomA;
    RTHandle _l2BloomB;
    FilteringSettings _filtering;

    sealed class DirectPassData
    {
        public TextureHandle color;
        public TextureHandle depth;
        public bool hasDepth;
        public bool bindDepth;
        public float debugMode;
        public RendererListHandle fxList;
        public bool hasFxList;
        public bool flushNameplates;
    }

}
