using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public sealed partial class L2FxCompositorRendererFeature : ScriptableRendererFeature
{
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
