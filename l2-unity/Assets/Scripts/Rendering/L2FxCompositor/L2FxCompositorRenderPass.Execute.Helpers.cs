using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public sealed partial class L2FxCompositorRenderPass
{
    void BindPostParams()
    {
        _postMaterial.SetFloat(BloomThresholdId, _settings.bloomThreshold);
        _postMaterial.SetFloat(BloomPreGainId, 1f);
        _postMaterial.SetFloat(BloomIntensityId, _settings.bloomIntensity);
        _postMaterial.SetFloat(FxGainId, _settings.fxGain);
        Shader.SetGlobalFloat(FxGainId, _settings.fxGain);
    }

    bool WantSkillPost()
    {
        return _settings != null && _settings.enableSkillPost && _postMaterial != null;
    }

    bool UseSkillPost()
    {
        return WantSkillPost() && _l2Scene != null && _l2BloomA != null && _l2BloomB != null;
    }

    bool ShouldRun(ref RenderingData renderingData)
    {
        return ShouldRun(renderingData.cameraData.camera);
    }

    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        Shader.SetGlobalFloat(D3D9ActiveId, 0f);
    }

    bool ShouldRun(Camera camera)
    {
        if (_settings == null || !_settings.enableD3D9Compositor)
            return false;

        CameraType type = camera.cameraType;
        if (_settings.gameCameraOnly &&
            type != CameraType.Game &&
            type != CameraType.VR)
        {
            return false;
        }

        return true;
    }
}
