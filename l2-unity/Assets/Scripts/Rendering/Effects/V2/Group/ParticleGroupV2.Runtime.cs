using System.Collections.Generic;
using UnityEngine;

public partial class ParticleGroupV2
{
    ParticleStreamRuntime EnsureRuntime()
    {
        if (_runtime != null)
        {
            return _runtime;
        }

        _slots ??= new ParticleSlotSet(this);
        _batch ??= new ParticleDrawBatch();
        _runtime = new ParticleStreamRuntime(this, _slots, _batch);
        return _runtime;
    }

    Renderer ResolveSlotRenderer()
    {
        if (_particles != null)
        {
            for (int i = 0; i < _particles.Length; i++)
            {
                if (_particles[i] != null)
                {
                    return _particles[i];
                }
            }
        }

        return GetComponentInChildren<Renderer>(true);
    }

    Material ResolveSlotMaterial()
    {
        if (_runtimeMaterialInstances != null)
        {
            for (int i = 0; i < _runtimeMaterialInstances.Length; i++)
            {
                if (_runtimeMaterialInstances[i] != null)
                {
                    return _runtimeMaterialInstances[i];
                }
            }
        }

        Renderer renderer = ResolveSlotRenderer();
        return renderer != null ? renderer.sharedMaterial : null;
    }

    void EnsureRuntimeMaterialInstances()
    {
        if (_runtimeMaterialInstances != null)
        {
            return;
        }

        Renderer[] renderers = ResolveSlotRenderers();
        if (renderers == null || renderers.Length == 0)
        {
            return;
        }

        var cloneBySource = new Dictionary<Material, Material>();
        var clones = new List<Material>();
        for (int rendererIndex = 0; rendererIndex < renderers.Length; rendererIndex++)
        {
            Renderer renderer = renderers[rendererIndex];
            if (renderer == null)
            {
                continue;
            }

            Material[] shared = renderer.sharedMaterials;
            Material[] instanceMaterials = new Material[shared.Length];
            for (int materialIndex = 0; materialIndex < shared.Length; materialIndex++)
            {
                Material source = shared[materialIndex];
                if (source == null)
                {
                    continue;
                }

                if (!cloneBySource.TryGetValue(source, out Material clone))
                {
                    clone = new Material(source)
                    {
                        name = source.name + " (runtime override)"
                    };
                    cloneBySource.Add(source, clone);
                    clones.Add(clone);
                }

                instanceMaterials[materialIndex] = clone;
            }

            renderer.sharedMaterials = instanceMaterials;
        }

        _runtimeMaterialInstances = clones.ToArray();
    }

    Renderer[] ResolveSlotRenderers()
    {
        if (_particles != null && _particles.Length > 0)
        {
            return _particles;
        }

        return GetComponentsInChildren<Renderer>(true);
    }

    void DestroyRuntimeMaterialInstances()
    {
        if (_runtimeMaterialInstances == null)
        {
            return;
        }

        for (int i = 0; i < _runtimeMaterialInstances.Length; i++)
        {
            if (_runtimeMaterialInstances[i] != null)
            {
                Destroy(_runtimeMaterialInstances[i]);
            }
        }

        _runtimeMaterialInstances = null;
    }

    void ResolveEmissionWindow(EffectSettings settings, MagicCastData castData)
    {
        if (_forceAuthoredTiming || _hostOwnedEmission)
        {
            _lifetimePolicy = ParticleLifetimePolicy.Authored;
            _emissionWindow = Mathf.Max(0.01f, _duration);
            return;
        }

        if (_hasExternalEmissionWindow)
        {
            _lifetimePolicy = ParticleLifetimePolicy.EmissionWindowFromCast;
            _emissionWindow = Mathf.Max(0.01f, _emissionWindow);
            return;
        }

        if (_hasFixedDuration)
        {
            _lifetimePolicy = ParticleLifetimePolicy.Authored;
            _emissionWindow = Mathf.Max(0.01f, _duration);
            return;
        }

        _lifetimePolicy = ParticleLifetimePolicy.EmissionWindowFromCast;
        float fallback = Mathf.Max(0.01f, _duration);
        _emissionWindow = EffectCastDurationResolver.Resolve(
            fallback,
            false,
            settings,
            castData,
            out _,
            out _);
        if (_emissionWindow < 0.01f)
        {
            _emissionWindow = fallback;
        }
    }
}
