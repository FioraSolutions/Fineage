using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;
using UnityEngine;

public sealed partial class L2TerrainOverlayRenderPass : ScriptableRenderPass
{
    static readonly ProfilingSampler Sampler = new ProfilingSampler("L2 Terrain Overlays");

    sealed class PassData
    {
        public TextureHandle color;
        public TextureHandle depth;
        public bool hasDepth;
    }


#if URP_COMPATIBILITY_MODE
#pragma warning disable 618, 672
#pragma warning restore 618, 672
#endif

}
