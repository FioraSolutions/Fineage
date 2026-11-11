using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public sealed partial class ParticleGroupSimulation
{
    public void PackAndDraw(Vector4 ownerWorldPos, Matrix4x4[] objectToWorldMatrices)
    {
        if (!TryPack(ownerWorldPos, objectToWorldMatrices, out int packed))
            return;

        _gpuDrawer.Draw(
            GpuMesh,
            GpuMaterials,
            GpuLayer,
            GpuRendererPriority,
            _packedSlots,
            _packedMatrices,
            packed);
    }

    void WriteGpuInstance(
        int slot,
        float shaderStartTime,
        float seed,
        uint meshBase,
        uint spriteBase,
        Vector4 spawnLocationAddUe)
    {
        if (!_gpuSlots.IsCreated || slot < 0 || slot >= _gpuSlots.Length)
            return;

        L2AppRand.ResolveGpuInstanceRandBits(
            HasMeshSpawn,
            HasStartSpin,
            meshBase,
            spriteBase,
            slot,
            out float meshSpawnRandBits,
            out float startSpinRandBits,
            out float spriteMotionRandBits,
            out float spriteSpinRandBits);

        L2FxParticleInstance instance = _gpuSlots[slot];
        instance.startTime = shaderStartTime;
        instance.seed = seed;
        instance.meshSpawnRandBits = meshSpawnRandBits;
        instance.startSpinRandBits = startSpinRandBits;
        instance.spriteMotionRandBits = spriteMotionRandBits;
        instance.spriteSpinRandBits = spriteSpinRandBits;
        instance.spawnLocationAddUe = spawnLocationAddUe;
        _gpuSlots[slot] = instance;
    }

    void EnsureGpu(int count)
    {
        if (count <= 0)
            return;

        if (_gpuSlots.IsCreated && _gpuSlots.Length == count)
            return;

        CompleteExpire();
        DisposeGpu();
        _gpuSlots = new NativeArray<L2FxParticleInstance>(count, Allocator.Persistent, NativeArrayOptions.ClearMemory);
        _packedSlots = new NativeArray<L2FxParticleInstance>(count, Allocator.Persistent, NativeArrayOptions.ClearMemory);
        _sourceMatrices = new NativeArray<Matrix4x4>(count, Allocator.Persistent, NativeArrayOptions.ClearMemory);
        _packedMatrices = new NativeArray<Matrix4x4>(count, Allocator.Persistent, NativeArrayOptions.ClearMemory);
        _packedCount = new NativeArray<int>(1, Allocator.Persistent, NativeArrayOptions.ClearMemory);
    }

    void DisposeGpu()
    {
        _gpuDrawer.Release();
        if (_gpuSlots.IsCreated)
            _gpuSlots.Dispose();
        if (_packedSlots.IsCreated)
            _packedSlots.Dispose();
        if (_sourceMatrices.IsCreated)
            _sourceMatrices.Dispose();
        if (_packedMatrices.IsCreated)
            _packedMatrices.Dispose();
        if (_packedCount.IsCreated)
            _packedCount.Dispose();
    }
}
