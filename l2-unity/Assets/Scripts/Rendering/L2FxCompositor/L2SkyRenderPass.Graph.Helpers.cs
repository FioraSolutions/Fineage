using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public sealed partial class L2SkyRenderPass
{
    bool TryCreateLayerList(
        RenderGraph renderGraph,
        UniversalRenderingData renderingData,
        UniversalCameraData cameraData,
        UniversalLightData lightData,
        LayerMask layerMask,
        RenderQueueRange queueRange,
        out RendererListHandle handle)
    {
        handle = default;
        if (layerMask.value == 0)
            return false;

        var filtering = new FilteringSettings(queueRange, layerMask);
        DrawingSettings drawing = RenderingUtils.CreateDrawingSettings(
            ForwardOnlyTag,
            renderingData,
            cameraData,
            lightData,
            SortingCriteria.CommonTransparent);
        drawing.perObjectData = PerObjectData.None;

        var param = new RendererListParams(renderingData.cullResults, drawing, filtering);
        handle = renderGraph.CreateRendererList(param);
        return handle.IsValid();
    }

    static RenderQueueRange QueueRange(int lower, int upper)
    {
        return new RenderQueueRange
        {
            lowerBound = lower,
            upperBound = upper
        };
    }

    public void Dispose()
    {
        CoreUtils.Destroy(_backdropMaterial);
        _backdropMaterial = null;
    }
}
