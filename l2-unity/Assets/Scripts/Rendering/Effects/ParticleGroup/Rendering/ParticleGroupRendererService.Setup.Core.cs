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
}
