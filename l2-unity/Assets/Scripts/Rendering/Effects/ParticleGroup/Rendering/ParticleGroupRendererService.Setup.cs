using System.Collections.Generic;
using UnityEngine;

public sealed partial class ParticleGroupRendererService
{
    public ParticleGroupRendererService(EffectPart host)
    {
        _host = host;
        _transform = host.transform;
    }

    public void SetParticles(Renderer[] particles)
    {
        RestoreCoordinateSystemSlots();
        _particles = particles;
        _transformOrigins = null;
    }

    public void CollectIfEmpty()
    {
        if (_particles == null || _particles.Length == 0)
            _particles = _transform.GetComponentsInChildren<Renderer>(true);
    }

    public void EnsureClones(bool cloneToMaxCount, int maxCount, int cloneLimit, ref bool clonesCreated)
    {
        if (!cloneToMaxCount || !Application.isPlaying || clonesCreated || _particles == null || _particles.Length == 0)
            return;

        int desiredCount = Mathf.Clamp(maxCount, _particles.Length, Mathf.Max(_particles.Length, cloneLimit));
        if (desiredCount <= _particles.Length)
            return;

        List<Renderer> particles = new List<Renderer>(_particles);
        int sourceCount = particles.Count;
        for (int i = particles.Count; i < desiredCount; i++)
        {
            Renderer source = particles[i % sourceCount];
            if (source == null)
                continue;

            GameObject clone = Object.Instantiate(source.gameObject, source.transform.parent);
            clone.name = $"{source.gameObject.name}_RuntimeClone";
            clone.SetActive(false);
            Renderer cloneRenderer = clone.GetComponent<Renderer>();
            if (cloneRenderer != null)
                particles.Add(cloneRenderer);
        }

        _particles = particles.ToArray();
        clonesCreated = true;
    }

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
