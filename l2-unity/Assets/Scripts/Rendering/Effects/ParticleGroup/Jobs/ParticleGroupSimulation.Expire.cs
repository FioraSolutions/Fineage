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

    public void CompleteExpire()
    {
        if (!_expireScheduled)
            return;

        _expireHandle.Complete();
        _expireScheduled = false;
        _expireHandle = default;
    }

    public void SyncManaged(bool[] managedActive, float[] managedTimes)
    {
        if (!_active.IsCreated || managedActive == null || managedTimes == null)
            return;

        int count = Mathf.Min(managedActive.Length, _active.Length);
        for (int i = 0; i < count; i++)
        {
            managedActive[i] = _active[i] != 0;
            managedTimes[i] = _spawnTimes[i];
        }
    }

    public void ClearAllActive(bool[] managedActive)
    {
        if (_active.IsCreated)
        {
            for (int i = 0; i < _active.Length; i++)
                _active[i] = 0;
        }

        if (managedActive == null)
            return;

        for (int i = 0; i < managedActive.Length; i++)
            managedActive[i] = false;
    }

    public void MarkSlotActive(int slot, float now, bool[] managedActive, float[] managedTimes)
    {
        if (managedTimes != null && slot < managedTimes.Length)
            managedTimes[slot] = now;
        if (managedActive != null && slot < managedActive.Length)
            managedActive[slot] = true;
        if (_spawnTimes.IsCreated && slot < _spawnTimes.Length)
            _spawnTimes[slot] = now;
        if (_active.IsCreated && slot < _active.Length)
            _active[slot] = 1;
    }

    void ClearLifetime()
    {
        if (!_active.IsCreated)
            return;

        for (int i = 0; i < _active.Length; i++)
        {
            _active[i] = 0;
            _spawnTimes[i] = 0f;
        }
    }

    void DisposeLifetime()
    {
        if (_spawnTimes.IsCreated)
            _spawnTimes.Dispose();
        if (_active.IsCreated)
            _active.Dispose();
    }
}
