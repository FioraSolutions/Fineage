using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

/// <summary>
/// Jobs facade: native slot buffers, expire/activate/pack Burst jobs, GPU bind flags.
/// Does not touch GameObject or Material APIs.
/// </summary>
public sealed partial class ParticleGroupSimulation
{
    readonly ParticleGroupGpuDrawer _gpuDrawer = new ParticleGroupGpuDrawer();
    NativeArray<float> _spawnTimes;
    NativeArray<byte> _active;
    NativeArray<L2FxParticleInstance> _gpuSlots;
    NativeArray<L2FxParticleInstance> _packedSlots;
    NativeArray<Matrix4x4> _sourceMatrices;
    NativeArray<Matrix4x4> _packedMatrices;
    NativeArray<int> _packedCount;
    JobHandle _expireHandle;
    bool _expireScheduled;

    public bool GpuEnabled { get; private set; }
    public Mesh GpuMesh { get; private set; }
    public Material[] GpuMaterials { get; private set; }
    public int GpuLayer { get; private set; }
    public int GpuRendererPriority { get; private set; }
    public bool HasMeshSpawn { get; private set; }
    public bool HasStartSpin { get; private set; }
    public bool HasLifetimeBuffers => _active.IsCreated;
    public NativeArray<L2FxParticleInstance> PackedSlots => _packedSlots;
    public NativeArray<Matrix4x4> PackedMatrices => _packedMatrices;

    public bool CanPackAndDraw =>
        GpuEnabled &&
        _gpuSlots.IsCreated &&
        _active.IsCreated &&
        _packedSlots.IsCreated &&
        _sourceMatrices.IsCreated &&
        _packedMatrices.IsCreated &&
        _packedCount.IsCreated;

}
