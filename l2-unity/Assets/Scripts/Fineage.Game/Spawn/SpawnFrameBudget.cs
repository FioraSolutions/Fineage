using System.Collections.Generic;
using UnityEngine;

public static partial class SpawnFrameBudget
{
    public const int MaxSpawnsPerFrame = 6;

    static readonly HashSet<int> Cancelled = new HashSet<int>();
    static int _frame = -1;
    static int _used;
    static int _spawned;
    static int _deferred;
    static int _reused;

    public static int Spawned { get { Sync(); return _spawned; } }
    public static int Deferred { get { Sync(); return _deferred; } }
    public static int Reused { get { Sync(); return _reused; } }



    /// <summary>
    /// True while this frame can still afford a spawn. Callers that walk a batch use this to stop
    /// without consuming budget, so one batch continuation replaces thousands of re-queued rows.
    /// </summary>





}
