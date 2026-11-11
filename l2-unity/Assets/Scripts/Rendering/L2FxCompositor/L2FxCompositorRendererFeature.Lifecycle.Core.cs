using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public sealed partial class L2FxCompositorRendererFeature : ScriptableRendererFeature
{
    public override void Create()
    {
        if (transferShader == null)
            transferShader = Shader.Find("Hidden/L2/FxColorTransfer");
        if (postShader == null)
            postShader = Shader.Find("Hidden/L2/FxPostBloomContrast");
        if (yebisShader == null)
            yebisShader = Shader.Find("Hidden/L2/YebisPost");

        if (transferShader != null)
        {
            if (_transferMaterial == null || _transferMaterial.shader != transferShader)
                _transferMaterial = CoreUtils.CreateEngineMaterial(transferShader);
        }

        if (postShader != null)
        {
            if (_postMaterial == null || _postMaterial.shader != postShader)
                _postMaterial = CoreUtils.CreateEngineMaterial(postShader);
        }

        if (yebisShader != null)
        {
            if (_yebisMaterial == null || _yebisMaterial.shader != yebisShader)
                _yebisMaterial = CoreUtils.CreateEngineMaterial(yebisShader);
        }

        _skyPass?.Dispose();
        _pass?.Dispose();
        _yebisPass?.Dispose();
        _skyPass = new L2SkyRenderPass(settings);
        _terrainOverlayPass = new L2TerrainOverlayRenderPass();
        _pass = new L2FxCompositorRenderPass(settings, _transferMaterial, _postMaterial)
        {
            renderPassEvent = settings != null
                ? settings.renderPassEvent
                : RenderPassEvent.BeforeRenderingPostProcessing
        };
        _yebisPass = new L2YebisPostRenderPass(settings, _yebisMaterial)
        {
            renderPassEvent = settings != null
                ? settings.renderPassEvent
                : RenderPassEvent.BeforeRenderingPostProcessing
        };

        bool prefer = settings != null && settings.enableD3D9Compositor;
        L2FxCompositorRuntime.SetPreferGpuQueue(prefer && isActive);
    }
}
