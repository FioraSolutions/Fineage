using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public sealed partial class L2FxCompositorRenderPass
{
    public L2FxCompositorRenderPass(
        L2FxCompositorSettings settings,
        Material transferMaterial,
        Material postMaterial)
    {
        _settings = settings;
        _transferMaterial = transferMaterial;
        _postMaterial = postMaterial;
        renderPassEvent = settings.renderPassEvent;
        _filtering = new FilteringSettings(RenderQueueRange.transparent, settings.effectLayerMask);
    }

    public void Dispose()
    {
        _l2Color?.Release();
        _l2Color = null;
        _l2Scene?.Release();
        _l2Scene = null;
        _l2Sky?.Release();
        _l2Sky = null;
        _l2BloomA?.Release();
        _l2BloomA = null;
        _l2BloomB?.Release();
        _l2BloomB = null;
    }
}
