using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public sealed partial class L2YebisPostRenderPass
{
    public L2YebisPostRenderPass(L2FxCompositorSettings settings, Material material)
    {
        _settings = settings;
        _material = material;
        renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public void Dispose()
    {
        _temp?.Release();
        _temp = null;
        _bloomA?.Release();
        _bloomA = null;
        _bloomB?.Release();
        _bloomB = null;
        _luma0?.Release();
        _luma0 = null;
        _luma1?.Release();
        _luma1 = null;
        _luma2?.Release();
        _luma2 = null;
        _adapted?.Release();
        _adapted = null;
        _adaptedPrev?.Release();
        _adaptedPrev = null;
        _sceneFloat?.Release();
        _sceneFloat = null;
        _bloomExclude?.Release();
        _bloomExclude = null;
        _captureScene?.Release();
        _captureScene = null;
        _captureGrade?.Release();
        _captureGrade = null;
        _captureControl?.Release();
        _captureControl = null;
        CoreUtils.Destroy(_excludeSkyMaterial);
        _excludeSkyMaterial = null;
        _adaptedSeeded = false;
        _execFrame = 0;
    }
}
