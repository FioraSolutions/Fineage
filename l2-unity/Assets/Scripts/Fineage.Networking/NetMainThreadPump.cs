using System;
using System.Collections.Concurrent;
using UnityEngine;

/// <summary>
/// Socket-thread → main-thread apply pump for Fineage Net (B3 budget / B4 extract).
/// Critical queue (despawn, inventory, die, teleport, …) drains first and is uncapped;
/// normal applies are capped per frame.
/// </summary>
public sealed class NetMainThreadPump
{
    /// <summary>Max zone packet applies per frame excluding critical queue traffic.</summary>
    public const int MaxNormalActionsPerFrame = 96;

    const int DepthWarnThreshold = 256;
    const float DepthLogCooldownSeconds = 5f;

    readonly ConcurrentQueue<Action> _critical = new ConcurrentQueue<Action>();
    readonly ConcurrentQueue<Action> _normal = new ConcurrentQueue<Action>();
    float _nextDepthLogTime;

    public int CriticalCount => _critical.Count;
    public int NormalCount => _normal.Count;
    public int Depth => _critical.Count + _normal.Count;

    public void Post(Action action)
    {
        if (action == null)
            return;
        _normal.Enqueue(action);
    }

    public void PostCritical(Action action)
    {
        if (action == null)
            return;
        _critical.Enqueue(action);
    }

    /// <summary>Drain critical fully, then up to <see cref="MaxNormalActionsPerFrame"/> normal actions.</summary>
    public void DrainFrame()
    {
        Drain(_critical, int.MaxValue);
        Drain(_normal, MaxNormalActionsPerFrame);
        MaybeWarnDepth();
    }

    static void Drain(ConcurrentQueue<Action> queue, int maxActions)
    {
        int remaining = maxActions;
        while (remaining > 0 && queue.TryDequeue(out Action action))
        {
            remaining--;
            try { action(); }
            catch (Exception ex) { Debug.LogException(ex); }
        }
    }

    void MaybeWarnDepth()
    {
        int depth = Depth;
        if (depth < DepthWarnThreshold)
            return;

        float now = Time.unscaledTime;
        if (now < _nextDepthLogTime)
            return;

        _nextDepthLogTime = now + DepthLogCooldownSeconds;
        Debug.LogWarning(
            $"[Net] main-thread packet queue depth {depth} (critical {_critical.Count}, normal {_normal.Count}); "
            + $"normal applies capped at {MaxNormalActionsPerFrame}/frame.");
    }
}
