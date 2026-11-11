using System;
using System.Collections.Generic;
using UnityEngine;

public abstract partial class TimedCompositeEffectBase : BaseEffect
{
    protected void InitializeTimedComposite(EffectSettings settings, MagicCastData castData)
    {
        _settings = settings;
        _castData = castData;
        _rootRuntimeSettings = CreateRuntimeSettings(settings);
    }

    protected EffectSettings CreateRuntimeSettings(EffectSettings sourceSettings, bool applyTimedLifetime = true)
    {
        if (sourceSettings == null)
        {
            return null;
        }

        EffectSettings runtime = Instantiate(sourceSettings);
        _runtimeSettings.Add(runtime);

        if (!applyTimedLifetime || _castData == null)
        {
            return runtime;
        }

        float timedLife = ResolveCastTimedLifetimeSeconds();
        if (timedLife <= 0f)
        {
            return runtime;
        }

        runtime.defaultLifeTime = timedLife;
        runtime.hideTime = Mathf.Min(runtime.hideTime, runtime.defaultLifeTime);

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        // Debug.Log(
            // $"{DebugPrefix} Runtime settings cloned. Source='{sourceSettings.name}' " +
            // $"timed={applyTimedLifetime} " +
            // $"hitTime={_castData.HitTime:F3}s animDur={_castData.SkillAnimationDuration:F3}s " +
            // $"tail={RuntimeLifeTimeTailSeconds:F3}s " +
            // $"lifeTime={runtime.defaultLifeTime:F3}s hideTime={runtime.hideTime:F3}s.");
#endif

        return runtime;
    }

    protected virtual float ResolveCastTimedLifetimeSeconds()
    {
        if (_castData == null || _castData.HitTime <= 0f)
        {
            return -1f;
        }

        return _castData.HitTime + Mathf.Max(0f, RuntimeLifeTimeTailSeconds);
    }

    protected EffectSettings SelectLifetimeSettings()
    {
        return _rootRuntimeSettings != null ? _rootRuntimeSettings : _settings;
    }
}
