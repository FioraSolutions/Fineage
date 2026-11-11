using System;
using UnityEngine;

public sealed partial class ParticleStreamRuntime
{
    public ParticleStreamRuntime(EffectPart host)
        : this(host, new ParticleSlotSet(host), new ParticleDrawBatch())
    {
    }

    public ParticleStreamRuntime(EffectPart host, ParticleSlotSet slots, ParticleDrawBatch batch)
    {
        _host = host;
        _slots = slots ?? new ParticleSlotSet(host);
        _batch = batch ?? new ParticleDrawBatch();
    }

    public void Bind(ParticleGroupAuthoring authoring)
    {
        _authoring = authoring;
        _slots.SetParticles(authoring.particles);
        _slots.CollectIfEmpty();
    }

    public void SetVisible(bool visible)
    {
        _visible = visible;
        if (visible && _batch.Bound)
        {
            _slots.DisableForGpuDraw();
        }
    }

    public void Stop(EmitterStopMode mode)
    {
        if (_state == EmitterState.Idle || _state == EmitterState.Complete)
        {
            return;
        }

        _state = EmitterState.Draining;
        if (mode == EmitterStopMode.Kill)
        {
            KillAll();
            _state = EmitterState.Complete;
        }
    }

    public void Dispose()
    {
        _simulation.CompleteExpire();
        _simulation.Dispose();
        _slots.RestoreCoordinateSystemSlots();
        _batch.Release();
    }
}
