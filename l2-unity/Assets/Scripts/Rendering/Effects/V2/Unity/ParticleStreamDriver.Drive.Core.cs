using UnityEngine;

public sealed partial class ParticleStreamDriver : EffectPart, IParticleEmitterV2
{
    void FixedUpdate()
    {
        if (!_streamVisible)
        {
            return;
        }

        _runtime?.Tick(Now());
    }

    void LateUpdate()
    {
        if (!_streamVisible)
        {
            return;
        }

        _runtime?.LateDraw();
    }

    void OnDisable()
    {
        if (_runtime == null)
        {
            return;
        }

        _runtime.Stop(_destroying ? EmitterStopMode.Kill : EmitterStopMode.Drain);
    }
}
