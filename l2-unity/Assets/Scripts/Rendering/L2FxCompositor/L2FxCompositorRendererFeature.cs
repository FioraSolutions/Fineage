using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public sealed partial class L2FxCompositorRendererFeature : ScriptableRendererFeature
{
    public L2FxCompositorSettings settings = new L2FxCompositorSettings();

    [SerializeField]
    Shader transferShader;

    [SerializeField]
    Shader postShader;

    [SerializeField]
    Shader yebisShader;

    Material _transferMaterial;
    Material _postMaterial;
    Material _yebisMaterial;
    L2SkyRenderPass _skyPass;
    L2FxCompositorRenderPass _pass;
    L2YebisPostRenderPass _yebisPass;
    L2TerrainOverlayRenderPass _terrainOverlayPass;




}
