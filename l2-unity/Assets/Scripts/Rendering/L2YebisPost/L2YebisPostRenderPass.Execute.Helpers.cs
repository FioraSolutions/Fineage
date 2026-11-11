using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public sealed partial class L2YebisPostRenderPass
{
    void FillBloomExcludeMask(CommandBuffer cmd, RTHandle cameraDepth)
    {
        if (_bloomExclude == null || cameraDepth == null)
        {
            BindBloomExclude(cmd, false);
            return;
        }

        EnsureExcludeSkyMaterial();
        CoreUtils.SetRenderTarget(cmd, _bloomExclude, cameraDepth, ClearFlag.Color, Color.black);

        if (_excludeSkyMaterial != null)
        {
            int skyPass = _excludeSkyMaterial.FindPass("BloomExclude");
            if (skyPass >= 0)
            {
                cmd.DrawProcedural(
                    Matrix4x4.identity,
                    _excludeSkyMaterial,
                    skyPass,
                    MeshTopology.Triangles,
                    3,
                    1);
            }
        }

        L2CloudSimple clouds = SkyPacketServices.Clouds;
        if (clouds != null)
            clouds.DrawBloomExclude(cmd);

        L2CelestialDiscs discs = SkyPacketServices.CelestialDiscs;
        if (discs != null)
        {
            discs.DrawSunBloomExclude(cmd);
            discs.DrawMoonBloomInclude(cmd);
        }

        BindBloomExclude(cmd, true);
    }

    void BindBloomExclude(CommandBuffer cmd, bool enabled)
    {
        float on = enabled && _bloomExclude != null ? 1f : 0f;
        _material.SetFloat(BloomExcludeId, on);
        cmd.SetGlobalFloat(BloomExcludeId, on);
        RenderTargetIdentifier mask = enabled && _bloomExclude != null
            ? _bloomExclude.nameID
            : (RenderTargetIdentifier)Texture2D.blackTexture;
        _material.SetTexture(BloomExcludeMaskId, enabled && _bloomExclude != null ? _bloomExclude.rt : Texture2D.blackTexture);
        cmd.SetGlobalTexture(BloomExcludeMaskId, mask);
    }

    void EnsureExcludeSkyMaterial()
    {
        if (_excludeSkyMaterial != null)
            return;

        Shader shader = Shader.Find("Hidden/L2/SkyBackdrop");
        if (shader != null)
            _excludeSkyMaterial = CoreUtils.CreateEngineMaterial(shader);
    }
}
