using UnityEngine;
using System;
using System.Text;

public abstract partial class BaseEffect
{
    public void BindLifetimeToHost()
    {
        _lifetimeOwnedByHost = true;
        CancelInvoke(nameof(BeginFadeOut));
    }

    public void BindLifetimeToProjectile()
    {
        _lifetimeOwnedByProjectile = true;
        CancelInvoke(nameof(BeginFadeOut));
    }

    public void BindLifetimeToAuthoredStreams()
    {
        _lifetimeOwnedByAuthoredStreams = true;
        CancelInvoke(nameof(BeginFadeOut));
    }

    public void PrepareDestroyOnHomeArrival()
    {
        _deferDestroyUntilHomeArrival = true;
        CancelInvoke(nameof(BeginFadeOut));
    }

    public void BeginHomeArrivalFade(float destroyDelaySeconds)
    {
        _deferDestroyUntilHomeArrival = false;
        CancelInvoke(nameof(BeginFadeOut));
        DisableContinuousLoopOnChildren();
        BeginFadeOut();
        float delay = Mathf.Max(0.05f, destroyDelaySeconds);

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        // Debug.Log(
            // $"[HOME_PROJECTILE] BeginHomeArrivalFade effect='{name}' destroyIn={delay:F3}s " +
            // $"childGroups={GetComponentsInChildren<ParticleGroup>(true).Length} " +
            // $"childSingles={GetComponentsInChildren<ParticleSingle>(true).Length}");
#endif

        Destroy(gameObject, delay);

        if (transform.parent != null && transform.parent.name == "HitPointProxy")
        {
            Destroy(transform.parent.gameObject, delay);
        }
    }

    public void CompleteHomeArrivalAndDestroy(float fadeSeconds = 0.35f)
    {
        BeginHomeArrivalFade(fadeSeconds);
    }

    public void DestroyHomeArrivalImmediate()
    {
        _deferDestroyUntilHomeArrival = false;
        CancelInvoke(nameof(BeginFadeOut));
        DisableContinuousLoopOnChildren();

        EffectPart[] parts = GetComponentsInChildren<EffectPart>(true);
        for (int i = 0; i < parts.Length; i++)
        {
            if (parts[i] != null)
            {
                parts[i].StopPart();
            }
        }

        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
            {
                renderers[i].enabled = false;
            }
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        // Debug.Log(
            // $"[HOME_PROJECTILE] DestroyHomeArrivalImmediate effect='{name}' " +
            // $"renderers={renderers.Length} parts={parts.Length}");
#endif

        Destroy(gameObject);

        if (transform.parent != null && transform.parent.name == "HitPointProxy")
        {
            Destroy(transform.parent.gameObject);
        }
    }

    private void DisableContinuousLoopOnChildren()
    {
        ParticleGroup[] groups = GetComponentsInChildren<ParticleGroup>(true);
        for (int i = 0; i < groups.Length; i++)
        {
            if (groups[i] != null)
            {
                groups[i].SetRuntimeContinuousLoopOverride(true, false);
            }
        }

        ParticleSingle[] singles = GetComponentsInChildren<ParticleSingle>(true);
        for (int i = 0; i < singles.Length; i++)
        {
            if (singles[i] != null)
            {
                singles[i].SetRuntimeContinuousLoopOverride(true, false);
            }
        }

        ParticleEmitterV2.StopAll(this);
    }
}
