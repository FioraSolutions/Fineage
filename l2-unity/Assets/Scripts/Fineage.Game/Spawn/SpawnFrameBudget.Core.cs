using System.Collections.Generic;
using UnityEngine;

public static partial class SpawnFrameBudget
{
    public static void BeginFrame()
    {
        int frame = Time.frameCount;
        if (_frame == frame)
            return;
        _frame = frame;
        _used = 0;
        _spawned = 0;
        _deferred = 0;
        _reused = 0;
    }

    public static bool TryConsumeSpawn()
    {
        Sync();
        if (_used >= MaxSpawnsPerFrame)
        {
            _deferred++;
            return false;
        }

        _used++;
        _spawned++;
        return true;
    }

    public static bool HasBudget()
    {
        Sync();
        return _used < MaxSpawnsPerFrame;
    }

    public static void NoteReused()
    {
        Sync();
        _reused++;
    }
}
