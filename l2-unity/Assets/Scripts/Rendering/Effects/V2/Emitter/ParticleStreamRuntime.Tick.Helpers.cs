using System;
using UnityEngine;

public sealed partial class ParticleStreamRuntime
{
    public void LateDraw()
    {
        if (!_visible ||
            _state == EmitterState.Idle ||
            _state == EmitterState.Complete ||
            !_batch.Bound)
        {
            return;
        }

        _simulation.CompleteExpire();
        Matrix4x4[] objectToWorldMatrices = _slots.ResolveObjectToWorldMatrices();
        _batch.Draw(
            _simulation,
            _slots.ResolveGpuOwnerWorldPos(_simulation.GpuMaterials),
            ResolveDrawMatrices(objectToWorldMatrices));
    }

    public float ReadParticleLifetime(float fallback)
    {
        return Mathf.Max(0.01f, _slots.ReadShaderSlotDuration(fallback));
    }

    public float ReadLifetimeCenter(float fallback)
    {
        return Mathf.Max(0.01f, _slots.ReadLifetimeCenter(fallback));
    }
}
