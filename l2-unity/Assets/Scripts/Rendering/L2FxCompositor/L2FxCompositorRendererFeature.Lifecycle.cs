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

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        bool prefer = settings != null && settings.enableD3D9Compositor && isActive;
        L2FxCompositorRuntime.SetPreferGpuQueue(prefer);

        if (_terrainOverlayPass != null)
            renderer.EnqueuePass(_terrainOverlayPass);

        bool yebis = WantYebis(ref renderingData);
        L2YebisPostRuntime.WillFlushNameplates = yebis;

        if (!prefer && !yebis)
            return;

        if (prefer)
        {
            bool allowCompositor = true;
            if (settings.gameCameraOnly)
            {
                CameraType type = renderingData.cameraData.camera.cameraType;
                if (type != CameraType.Game && type != CameraType.VR)
                    allowCompositor = false;
            }

            if (allowCompositor)
            {
                if (!settings.directCameraColor && _transferMaterial == null)
                {
                    if (!yebis)
                        return;
                }
                else
                {
                    if (settings.directCameraColor)
                        renderer.EnqueuePass(_skyPass);

                    _pass.renderPassEvent = settings.renderPassEvent;
                    renderer.EnqueuePass(_pass);
                }
            }
        }

        if (yebis && _yebisPass != null && _yebisMaterial != null)
        {
            _yebisPass.renderPassEvent = settings.renderPassEvent;
            renderer.EnqueuePass(_yebisPass);
        }
    }
}
