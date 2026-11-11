using UnityEngine;
using Unity.Jobs;

public partial class ParticleGroup
{
    void LateUpdate()
    {
        if (_playback.Stopped || !_simulation.CanPackAndDraw)
            return;

        _simulation.CompleteExpire();
        _simulation.PackAndDraw(
            Renderers.ResolveGpuOwnerWorldPos(_simulation.GpuMaterials),
            Renderers.ResolveObjectToWorldMatrices());
    }

    void OnDisable()
    {
        ParticleGroupJobPump.Remove(this);
        _simulation.CompleteExpire();
        if (_isParticleActive != null)
            Renderers.ApplyExpire(_simulation, _isParticleActive, _particleSpawnTimes);
    }

    void OnDestroy()
    {
        ParticleGroupJobPump.Remove(this);
        _simulation.Dispose();
    }
}
