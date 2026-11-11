using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Reconstructs the original L2 colour-pass sky.
/// Upper sky / sun / clouds stay behind world depth. Hard lower haze
/// matches EID 1475 and fills the whole below-horizon buffer.
/// </summary>
public sealed partial class L2SkyRenderPass : ScriptableRenderPass
{
    static readonly int D3D9ActiveId = Shader.PropertyToID("_L2FxD3D9CompositorActive");
    static readonly int FxGainId = Shader.PropertyToID("_FxGain");
    static readonly int SkyColorId = Shader.PropertyToID("_SkyColor");
    static readonly int LowerHazeColorId = Shader.PropertyToID("_LowerHazeColor");
    static readonly ShaderTagId ForwardOnlyTag = new ShaderTagId("UniversalForwardOnly");
    static readonly ProfilingSampler Sampler = new ProfilingSampler("L2 Original Sky Layers");

    readonly L2FxCompositorSettings _settings;
    Material _backdropMaterial;

    sealed class PassData
    {
        public TextureHandle color;
        public TextureHandle depth;
        public bool hasDepth;
        public Material backdrop;
        public Color skyColor;
        public Color hazeColor;
        public RendererListHandle celestialHigh;
        public RendererListHandle celestialLow;
        public RendererListHandle clouds;
        public RendererListHandle haze;
        public bool hasCelestialHigh;
        public bool hasCelestialLow;
        public bool hasClouds;
        public bool hasHaze;
    }

}
