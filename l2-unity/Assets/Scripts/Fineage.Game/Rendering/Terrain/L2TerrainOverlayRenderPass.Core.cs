using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public sealed partial class L2TerrainOverlayRenderPass : ScriptableRenderPass
{
    public L2TerrainOverlayRenderPass()
    {
        renderPassEvent = RenderPassEvent.AfterRenderingOpaques + 1;
        ConfigureInput(ScriptableRenderPassInput.Color | ScriptableRenderPassInput.Depth);
    }
}
