using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public sealed partial class ParticleGroupSimulation
{
    public bool TryBindGpu(Renderer[] particles, bool useGpuInstancing)
    {
        GpuEnabled = false;
        GpuMesh = null;
        GpuMaterials = null;
        GpuRendererPriority = 0;
        HasMeshSpawn = false;
        HasStartSpin = false;
        DisposeGpu();

        if (!useGpuInstancing || particles == null || particles.Length == 0)
            return false;

        if (!ParticleGroupGpuDrawer.TryBind(
                particles,
                out Mesh mesh,
                out Material[] materials,
                out int layer,
                out int rendererPriority))
            return false;

        GpuEnabled = true;
        GpuMesh = mesh;
        GpuMaterials = materials;
        GpuLayer = layer;
        GpuRendererPriority = rendererPriority;
        Material shared = materials != null && materials.Length > 0 ? materials[0] : null;
        HasMeshSpawn = L2MaterialPropertyCopier.IsMeshSpawnParticleMaterial(shared);
        HasStartSpin = L2MaterialPropertyCopier.IsMeshStartSpinMaterial(shared);
        EnsureGpu(particles.Length);
        return true;
    }

    public void ActivateGpuSlot(
        int slot,
        float now,
        float shaderStartTime,
        float seed,
        uint meshBase,
        uint spriteBase,
        bool[] managedActive,
        float[] managedTimes,
        Vector4 spawnLocationAddUe = default)
    {
        MarkSlotActive(slot, now, managedActive, managedTimes);
        WriteGpuInstance(slot, shaderStartTime, seed, meshBase, spriteBase, spawnLocationAddUe);
    }

    public bool TryActivateGpuBurst(
        float now,
        float warmup,
        int maxCount,
        bool skipRestart,
        uint meshBase,
        uint spriteBase,
        ref int particleIndex,
        bool[] managedActive,
        float[] managedTimes)
    {
        if (!GpuEnabled || !_gpuSlots.IsCreated || maxCount < ParticleGroupSimJobs.GpuBurstJobMinSlots)
            return false;

        EnsureLifetime(_gpuSlots.Length);
        NativeArray<float> seeds = new NativeArray<float>(maxCount, Allocator.TempJob);
        try
        {
            for (int i = 0; i < maxCount; i++)
                seeds[i] = Random.Range(-100f, 100f);

            float shaderStartTime = now - warmup;
            bool canParallel = !skipRestart &&
                               particleIndex >= 0 &&
                               particleIndex + maxCount <= _gpuSlots.Length;

            if (canParallel)
            {
                new ParticleGroupSimJobs.ActivateGpuParallelJob
                {
                    slots = _gpuSlots,
                    active = _active,
                    spawnTimes = _spawnTimes,
                    seeds = seeds,
                    now = now,
                    shaderStartTime = shaderStartTime,
                    meshBase = meshBase,
                    spriteBase = spriteBase,
                    hasMeshSpawn = (byte)(HasMeshSpawn ? 1 : 0),
                    hasStartSpin = (byte)(HasStartSpin ? 1 : 0),
                    startIndex = particleIndex
                }.Schedule(maxCount, 32).Complete();
                particleIndex += maxCount;
            }
            else
            {
                new ParticleGroupSimJobs.ActivateGpuSequentialJob
                {
                    slots = _gpuSlots,
                    active = _active,
                    spawnTimes = _spawnTimes,
                    seeds = seeds,
                    now = now,
                    shaderStartTime = shaderStartTime,
                    meshBase = meshBase,
                    spriteBase = spriteBase,
                    hasMeshSpawn = (byte)(HasMeshSpawn ? 1 : 0),
                    hasStartSpin = (byte)(HasStartSpin ? 1 : 0),
                    startIndex = particleIndex,
                    activateCount = maxCount,
                    skipRestartIfActive = (byte)(skipRestart ? 1 : 0)
                }.Schedule().Complete();
                for (int i = 0; i < maxCount; i++)
                {
                    if (particleIndex >= _gpuSlots.Length)
                        particleIndex = 0;
                    particleIndex++;
                }
            }

            SyncManaged(managedActive, managedTimes);
            return true;
        }
        finally
        {
            if (seeds.IsCreated)
                seeds.Dispose();
        }
    }

    public bool TryPack(Vector4 ownerWorldPos, Matrix4x4[] objectToWorldMatrices, out int packed)
    {
        packed = 0;
        if (!CanPackAndDraw ||
            objectToWorldMatrices == null ||
            objectToWorldMatrices.Length < _active.Length)
            return false;

        for (int i = 0; i < _active.Length; i++)
            _sourceMatrices[i] = objectToWorldMatrices[i];

        new ParticleGroupSimJobs.PackGpuJob
        {
            slots = _gpuSlots,
            active = _active,
            sourceMatrices = _sourceMatrices,
            packed = _packedSlots,
            matrices = _packedMatrices,
            packedCount = _packedCount,
            ownerWorldPos = ownerWorldPos,
            count = _active.Length
        }.Run();

        packed = _packedCount[0];
        return packed > 0;
    }

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
