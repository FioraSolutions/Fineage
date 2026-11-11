using System.Collections.Generic;
using UnityEngine;

public sealed partial class ParticleGroupRendererService
{
    public void ExpandShaderDrivenBounds()
    {
        if (_particles == null)
            return;

        for (int i = 0; i < _particles.Length; i++)
        {
            Renderer renderer = _particles[i];
            if (renderer == null)
                continue;

            bool expand = false;
            Material[] materials = renderer.sharedMaterials;
            if (materials != null)
            {
                for (int m = 0; m < materials.Length; m++)
                {
                    Material material = materials[m];
                    if (material != null &&
                        material.HasProperty("_ExpandShaderBounds") &&
                        material.GetFloat("_ExpandShaderBounds") > 0.5f)
                    {
                        expand = true;
                        break;
                    }
                }
            }

            if (!expand)
                continue;

            renderer.allowOcclusionWhenDynamic = false;
            renderer.localBounds = new Bounds(
                new Vector3(0f, 0.5f, 0f),
                new Vector3(8f, 8f, 8f));
        }
    }

    public void HideAll()
    {
        if (_particles == null)
            return;

        for (int i = 0; i < _particles.Length; i++)
        {
            if (_particles[i] != null)
                _particles[i].gameObject.SetActive(false);
        }
    }

    public void DisableForGpuDraw()
    {
        if (_particles == null)
            return;

        for (int i = 0; i < _particles.Length; i++)
        {
            if (_particles[i] == null)
                continue;
            _particles[i].enabled = false;
            _particles[i].gameObject.SetActive(false);
        }
    }

    public void EnableForGameObjectDraw()
    {
        if (_particles == null)
            return;

        for (int i = 0; i < _particles.Length; i++)
        {
            if (_particles[i] != null)
                _particles[i].enabled = true;
        }
    }
}
