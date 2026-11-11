using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public sealed partial class ParticleGroupSimulation
{
    public bool IsActive(int slot) =>
        _active.IsCreated && slot >= 0 && slot < _active.Length && _active[slot] != 0;

    public float SpawnTime(int slot) =>
        _spawnTimes.IsCreated && slot >= 0 && slot < _spawnTimes.Length ? _spawnTimes[slot] : 0f;

    public void EnsureLifetime(int count, bool reset = false)
    {
        if (count <= 0)
            return;

        if (_spawnTimes.IsCreated && _spawnTimes.Length == count &&
            _active.IsCreated && _active.Length == count)
        {
            if (reset)
                ClearLifetime();
            return;
        }

        CompleteExpire();
        DisposeLifetime();
        _spawnTimes = new NativeArray<float>(count, Allocator.Persistent, NativeArrayOptions.ClearMemory);
        _active = new NativeArray<byte>(count, Allocator.Persistent, NativeArrayOptions.ClearMemory);
    }

    public void ScheduleExpire(float now, float duration)
    {
        CompleteExpire();
        if (!_active.IsCreated)
            return;

        var job = new ParticleGroupSimJobs.ExpireJob
        {
            spawnTimes = _spawnTimes,
            active = _active,
            now = now,
            duration = duration,
            count = _active.Length
        };

        if (_active.Length >= ParticleGroupSimJobs.ExpireJobMinSlots)
        {
            _expireHandle = job.Schedule();
            _expireScheduled = true;
            return;
        }

        job.Run();
        _expireScheduled = false;
        _expireHandle = default;
    }

    public JobHandle ConsumeExpireHandle()
    {
        if (!_expireScheduled)
            return default;

        _expireScheduled = false;
        return _expireHandle;
    }
}
